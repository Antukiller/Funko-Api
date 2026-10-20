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

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Id, Is.GreaterThan(0));
        Assert.That(result.Value.Name, Is.EqualTo("Funko de prueba"));
        Assert.That(result.Value.Categoria.Id, Is.EqualTo(2));
        Assert.That(result.Value.IsDeleted, Is.False);
        Assert.That(result.Value.CreatedAt.Kind, Is.EqualTo(DateTimeKind.Utc));
        Assert.That(result.Value.UpdatedAt, Is.EqualTo(result.Value.CreatedAt));
    }

    [Test]
    public async Task CreateAsync_ConCategoriaInexistente_RetornaNotFound() {
        var result = await _repository.CreateAsync(CrearFunko("Funko inválido", 999));

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<NotFound>());
        Assert.That(result.Error.Message, Does.Contain("999"));
    }

    [Test]
    public async Task GetAllAsync_SinFiltros_RetornaFunkosConCategoria() {
        var funkos = await _repository.GetAllAsync();

        Assert.That(funkos, Has.Count.EqualTo(5));
        Assert.That(funkos.All(funko => funko.Categoria is not null), Is.True);
    }

    [Test]
    public async Task GetByIdAsync_CuandoExiste_RetornaFunko() {
        var result = await _repository.GetByIdAsync(1);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Name, Is.EqualTo("Naruto Uzumaki"));
        Assert.That(result.Value.Categoria.Name, Is.EqualTo("Anime"));
    }

    [Test]
    public async Task GetByIdAsync_CuandoNoExiste_RetornaNotFound() {
        var result = await _repository.GetByIdAsync(999);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<NotFound>());
        Assert.That(result.Error.Message, Does.Contain("999"));
    }

    [Test]
    public async Task UpdateAsync_ConDatosValidos_ActualizaFunkoExistente() {
        var funko = CrearFunko("Funko actualizado", 2);

        var result = await _repository.UpdateAsync(1, funko);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Id, Is.EqualTo(1));
        Assert.That(result.Value.Name, Is.EqualTo("Funko actualizado"));
        Assert.That(result.Value.Price, Is.EqualTo(19.99m));
        Assert.That(result.Value.Stock, Is.EqualTo(3));
        Assert.That(result.Value.Categoria.Id, Is.EqualTo(2));
        Assert.That(result.Value.UpdatedAt.Kind, Is.EqualTo(DateTimeKind.Utc));
    }

    [Test]
    public async Task UpdateAsync_CuandoNoExiste_RetornaNotFound() {
        var result = await _repository.UpdateAsync(999, CrearFunko("Funko", 1));

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<NotFound>());
    }

    [Test]
    public async Task UpdateAsync_CuandoCategoriaNoExiste_RetornaNotFound() {
        var result = await _repository.UpdateAsync(1, CrearFunko("Funko", 999));

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<NotFound>());
        Assert.That(result.Error.Message, Does.Contain("999"));
    }

    [Test]
    public async Task UpdateAsync_CuandoFunkoDeEntradaEstaBorrado_RetornaNotFound() {
        var funko = CrearFunko("Funko eliminado", 1);
        funko.IsDeleted = true;

        var result = await _repository.UpdateAsync(1, funko);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<NotFound>());
    }

    [Test]
    public async Task DeleteAsync_CuandoExiste_BorraLogicamente() {
        var deleted = await _repository.DeleteAsync(1);

        Assert.That(deleted, Is.True);
        Assert.That((await _repository.GetByIdAsync(1)).IsFailure, Is.True);
        Assert.That(await _context.Funkos.IgnoreQueryFilters().AnyAsync(funko => funko.Id == 1 && funko.IsDeleted), Is.True);
    }

    [Test]
    public async Task DeleteAsync_CuandoNoExiste_RetornaFalse() {
        var deleted = await _repository.DeleteAsync(999);

        Assert.That(deleted, Is.False);
    }

    private static Funko CrearFunko(string name, long categoriaId) => new() {
        Name = name,
        Price = 19.99m,
        Stock = 3,
        Categoria = new Categoria { Id = categoriaId }
    };
}
