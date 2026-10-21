using FluentAssertions;
using Funko_Api.Data;
using Funko_Api.Errors;
using Funko_Api.Models;
using Funko_Api.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Funko_Api.Test.Repositories;

[TestFixture]
public class FunkoRepositoryTests {
    private SqliteConnection _connection = null!;
    private AppDbContext _context = null!;
    private FunkoRepository _repository = null!;

    [SetUp]
    public void SetUp() {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new FunkoRepository(_context);
    }

    [TearDown]
    public void TearDown() {
        _context.Dispose();
        _connection.Dispose();
    }

    [Test]
    public async Task CreateAsync_ConDatosValidos_CreaFunkoConFechasYCategoria() {
        var funko = CrearFunko("Funko de prueba", 2);

        var result = await _repository.CreateAsync(funko);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().BePositive();
        result.Value.Name.Should().Be("Funko de prueba");
        result.Value.Categoria.Id.Should().Be(2);
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
    public async Task GetAllAsync_SinFiltros_RetornaFunkosConCategoria() {
        var funkos = await _repository.GetAllAsync();

        funkos.Should().HaveCount(5);
        funkos.Should().OnlyContain(funko => funko.Categoria != null);
    }

    [Test]
    public async Task GetByIdAsync_CuandoExiste_RetornaFunko() {
        var result = await _repository.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Naruto Uzumaki");
        result.Value.Categoria.Name.Should().Be("Anime");
    }

    [Test]
    public async Task GetByIdAsync_CuandoNoExiste_RetornaNotFound() {
        var result = await _repository.GetByIdAsync(999);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
        result.Error.Message.Should().Contain("999");
    }

    [Test]
    public async Task UpdateAsync_ConDatosValidos_ActualizaFunkoExistente() {
        var funko = CrearFunko("Funko actualizado", 2);

        var result = await _repository.UpdateAsync(1, funko);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(1);
        result.Value.Name.Should().Be("Funko actualizado");
        result.Value.Price.Should().Be(19.99m);
        result.Value.Stock.Should().Be(3);
        result.Value.Categoria.Id.Should().Be(2);
        result.Value.UpdatedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public async Task UpdateAsync_CuandoNoExiste_RetornaNotFound() {
        var result = await _repository.UpdateAsync(999, CrearFunko("Funko", 1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
    }

    [Test]
    public async Task UpdateAsync_CuandoCategoriaNoExiste_RetornaNotFound() {
        var result = await _repository.UpdateAsync(1, CrearFunko("Funko", 999));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
        result.Error.Message.Should().Contain("999");
    }

    [Test]
    public async Task UpdateAsync_CuandoFunkoDeEntradaEstaBorrado_RetornaNotFound() {
        var funko = CrearFunko("Funko eliminado", 1);
        funko.IsDeleted = true;

        var result = await _repository.UpdateAsync(1, funko);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
    }

    [Test]
    public async Task DeleteAsync_CuandoExiste_BorraLogicamente() {
        var deleted = await _repository.DeleteAsync(1);

        deleted.Should().BeTrue();
        (await _repository.GetByIdAsync(1)).IsFailure.Should().BeTrue();
        (await _context.Funkos.IgnoreQueryFilters()
            .AnyAsync(funko => funko.Id == 1 && funko.IsDeleted)).Should().BeTrue();
    }

    [Test]
    public async Task DeleteAsync_CuandoNoExiste_RetornaFalse() {
        var deleted = await _repository.DeleteAsync(999);

        deleted.Should().BeFalse();
    }

    private static Funko CrearFunko(string name, long categoriaId) => new() {
        Name = name,
        Price = 19.99m,
        Stock = 3,
        Categoria = new Categoria { Id = categoriaId }
    };
}
