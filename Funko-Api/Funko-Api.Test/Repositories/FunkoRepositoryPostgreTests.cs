using FluentAssertions;
using Funko_Api.Data;
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
        var creado = await _repository.CreateAsync(await CrearFunkoAsync("Funko de prueba", 2));

        creado.Id.Should().BeGreaterThan(5);
        creado.Name.Should().Be("Funko de prueba");
        creado.Categoria.Name.Should().Be("Videojuegos");
        creado.IsDeleted.Should().BeFalse();
        creado.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        creado.UpdatedAt.Should().Be(creado.CreatedAt);
    }

    [Test]
    public async Task GetAllAsync_ConSemillas_RetornaFunkosIncluyendoCategoria() {
        var funkos = await _repository.GetAllAsync();

        funkos.Should().HaveCount(5);
        funkos.Should().OnlyContain(funko => funko.Categoria != null);
    }

    [Test]
    public async Task GetByIdAsync_Existente_RetornaFunkoConCategoria() {
        var funko = await _repository.GetByIdAsync(1);

        funko.Should().NotBeNull();
        funko!.Name.Should().Be("Naruto Uzumaki");
        funko.Categoria.Name.Should().Be("Anime");
    }

    [Test]
    public async Task GetByIdAsync_Inexistente_RetornaNull() {
        var funko = await _repository.GetByIdAsync(9999);

        funko.Should().BeNull();
    }

    [Test]
    public async Task UpdateAsync_Existente_PersisteCambiosEnPostgreSql() {
        var actualizado = await _repository.UpdateAsync(1, await CrearFunkoAsync("Nombre actualizado", 2));

        actualizado.Should().NotBeNull();
        actualizado!.Name.Should().Be("Nombre actualizado");
        actualizado.Price.Should().Be(19.99m);
        actualizado.Stock.Should().Be(3);
        actualizado.Categoria.Name.Should().Be("Videojuegos");

        var releido = await _repository.GetByIdAsync(1);
        releido.Should().NotBeNull();
        releido!.Name.Should().Be("Nombre actualizado");
        releido.Categoria.Name.Should().Be("Videojuegos");
    }

    [Test]
    public async Task UpdateAsync_Inexistente_RetornaNull() {
        var actualizado = await _repository.UpdateAsync(9999, await CrearFunkoAsync("Funko", 1));

        actualizado.Should().BeNull();
    }

    [Test]
    public async Task UpdateAsync_CuandoEstaBorrado_RetornaNull() {
        await _repository.DeleteAsync(1);

        var actualizado = await _repository.UpdateAsync(1, await CrearFunkoAsync("Funko", 1));

        actualizado.Should().BeNull();
    }

    [Test]
    public async Task DeleteAsync_Existente_MarcaBorradoLogicoYLoOculta() {
        var deleted = await _repository.DeleteAsync(1);

        deleted.Should().BeTrue();
        (await _repository.GetByIdAsync(1)).Should().BeNull();

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

    [Test]
    public async Task ExistsByNameAsync_ConExcludeId_NoCuentaElMismoFunko() {
        var exists = await _repository.ExistsByNameAsync("Naruto Uzumaki", 1);

        exists.Should().BeFalse();
    }

    [Test]
    public async Task UpdateStockAsync_Existente_PersisteCambiosEnPostgreSql() {
        var actualizado = await _repository.UpdateStockAsync(1, 3);

        actualizado.Should().NotBeNull();
        actualizado!.Stock.Should().Be(3);

        var releido = await _repository.GetByIdAsync(1);
        releido!.Stock.Should().Be(3);
    }

    private async Task<Funko> CrearFunkoAsync(string name, long categoriaId) {
        var categoria = await _context.Categorias.FindAsync(categoriaId);
        return new Funko {
            Name = name,
            Price = 19.99m,
            Stock = 3,
            Categoria = categoria!
        };
    }
}
