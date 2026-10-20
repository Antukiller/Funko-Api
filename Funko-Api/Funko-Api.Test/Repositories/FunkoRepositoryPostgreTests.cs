using FluentAssertions;
using Funko_Api.Data;
using Funko_Api.Errors;
using Funko_Api.Models;
using Funko_Api.Repositories;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Funko_Api.Test.Repositories;

[TestFixture]
public sealed class FunkoRepositoryPostgreTests {
    private PostgreSqlContainer _container = null!;
    private AppDbContext _context = null!;
    private FunkoRepository _repository = null!;

    [OneTimeSetUp]
    public async Task OneTimeSetUp() {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("funkos_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await _container.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown() {
        if (_container is not null)
            await _container.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp() {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        _context = new AppDbContext(options);
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();

        // EnsureCreated inserts explicit seed IDs without advancing PostgreSQL's identity sequence.
        await _context.Database.ExecuteSqlRawAsync(
            """SELECT setval(pg_get_serial_sequence('funko', 'Id'), (SELECT MAX("Id") FROM funko))""");

        _repository = new FunkoRepository(_context);
    }

    [TearDown]
    public async Task TearDown() {
        if (_context is not null)
            await _context.DisposeAsync();
    }

    [Test]
    public async Task CreateAsync_ConCategoriaValida_CreaFunkoConIdGenerado() {
        var result = await _repository.CreateAsync(CrearFunko("Funko de prueba", 2));

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().BeGreaterThan(5);
        result.Value.Name.Should().Be("Funko de prueba");
        result.Value.Categoria.Name.Should().Be("Videojuegos");
        result.Value.IsDeleted.Should().BeFalse();
        result.Value.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        result.Value.UpdatedAt.Should().Be(result.Value.CreatedAt);
    }

    [Test]
    public async Task CreateAsync_ConCategoriaInexistente_RetornaNotFound() {
        var result = await _repository.CreateAsync(CrearFunko("Funko inválido", 999));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
        result.Error.Message.Should().Contain("999");
    }

    [Test]
    public async Task GetAllAsync_ConSemillas_RetornaFunkosIncluyendoCategoria() {
        var funkos = await _repository.GetAllAsync();

        funkos.Should().HaveCount(5);
        funkos.Should().OnlyContain(funko => funko.Categoria != null);
    }

    [Test]
    public async Task GetByIdAsync_Existente_RetornaFunkoConCategoria() {
        var result = await _repository.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Naruto Uzumaki");
        result.Value.Categoria.Name.Should().Be("Anime");
    }

    [Test]
    public async Task GetByIdAsync_Inexistente_RetornaNotFound() {
        var result = await _repository.GetByIdAsync(9999);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
    }

    [Test]
    public async Task UpdateAsync_Existente_PersisteCambiosEnPostgreSql() {
        var result = await _repository.UpdateAsync(1, CrearFunko("Nombre actualizado", 2));

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Nombre actualizado");
        result.Value.Price.Should().Be(19.99m);
        result.Value.Stock.Should().Be(3);
        result.Value.Categoria.Name.Should().Be("Videojuegos");

        var reread = await _repository.GetByIdAsync(1);
        reread.IsSuccess.Should().BeTrue();
        reread.Value.Name.Should().Be("Nombre actualizado");
        reread.Value.Categoria.Name.Should().Be("Videojuegos");
    }

    [Test]
    public async Task UpdateAsync_Inexistente_RetornaNotFound() {
        var result = await _repository.UpdateAsync(9999, CrearFunko("Funko", 1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
    }

    [Test]
    public async Task UpdateAsync_ConCategoriaInexistente_RetornaNotFound() {
        var result = await _repository.UpdateAsync(1, CrearFunko("Funko", 999));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
        result.Error.Message.Should().Contain("999");
    }

    [Test]
    public async Task UpdateAsync_ConFunkoMarcadoComoEliminado_RetornaNotFound() {
        var funko = CrearFunko("Funko eliminado", 1);
        funko.IsDeleted = true;

        var result = await _repository.UpdateAsync(1, funko);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
    }

    [Test]
    public async Task DeleteAsync_Existente_MarcaBorradoLogicoYLoOculta() {
        var deleted = await _repository.DeleteAsync(1);

        deleted.Should().BeTrue();
        (await _repository.GetByIdAsync(1)).IsFailure.Should().BeTrue();

        var row = await _context.Funkos
            .IgnoreQueryFilters()
            .AsNoTracking()
            .SingleAsync(funko => funko.Id == 1);
        row.IsDeleted.Should().BeTrue();
    }

    [Test]
    public async Task DeleteAsync_Inexistente_RetornaFalse() {
        var deleted = await _repository.DeleteAsync(9999);

        deleted.Should().BeFalse();
    }

    private static Funko CrearFunko(string name, long categoriaId) => new() {
        Name = name,
        Price = 19.99m,
        Stock = 3,
        Categoria = new Categoria { Id = categoriaId }
    };
}
