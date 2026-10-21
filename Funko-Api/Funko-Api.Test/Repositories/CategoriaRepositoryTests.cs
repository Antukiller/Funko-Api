using FluentAssertions;
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

        categorias.Should().HaveCount(3);
        categorias.Select(categoria => categoria.Name)
            .Should().BeEquivalentTo(new[] { "Anime", "Videojuegos", "Cómics" });
    }

    [Test]
    public async Task GetByIdAsync_CuandoExiste_RetornaCategoria() {
        var result = await _repository.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(1);
        result.Value.Name.Should().Be("Anime");
    }

    [Test]
    public async Task GetByIdAsync_CuandoNoExiste_RetornaNotFound() {
        var result = await _repository.GetByIdAsync(999);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>();
        result.Error.Message.Should().Contain("Categoría").And.Contain("999");
    }
}
