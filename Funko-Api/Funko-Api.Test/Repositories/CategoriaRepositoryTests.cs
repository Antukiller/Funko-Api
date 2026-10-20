using Funko_Api.Data;
using Funko_Api.Errors;
using Funko_Api.Repositories.Categoria;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Funko_Api.Test.Repositories;

[TestFixture]
public class CategoriaRepositoryTests {
    private SqliteConnection _connection = null!;
    private AppDbContext _context = null!;
    private CategoriaRepository _repository = null!;

    [SetUp]
    public void SetUp() {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new AppDbContext(options);
        _context.Database.EnsureCreated();
        _repository = new CategoriaRepository(_context);
    }

    [TearDown]
    public void TearDown() {
        _context.Dispose();
        _connection.Dispose();
    }

    [Test]
    public async Task GetAllAsync_ConCategoriasSembradas_RetornaTodas() {
        var categorias = await _repository.GetAllAsync();

        Assert.That(categorias, Has.Count.EqualTo(3));
        Assert.That(categorias.Select(categoria => categoria.Name),
            Is.EquivalentTo(new[] { "Anime", "Videojuegos", "Cómics" }));
    }

    [Test]
    public async Task GetByIdAsync_CuandoExiste_RetornaCategoria() {
        var result = await _repository.GetByIdAsync(1);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value.Id, Is.EqualTo(1));
        Assert.That(result.Value.Name, Is.EqualTo("Anime"));
    }

    [Test]
    public async Task GetByIdAsync_CuandoNoExiste_RetornaNotFound() {
        var result = await _repository.GetByIdAsync(999);

        Assert.That(result.IsFailure, Is.True);
        Assert.That(result.Error, Is.TypeOf<NotFound>());
        Assert.That(result.Error.Message, Does.Contain("Categoría"));
        Assert.That(result.Error.Message, Does.Contain("999"));
    }
}
