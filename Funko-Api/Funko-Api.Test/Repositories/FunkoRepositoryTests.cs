using FluentAssertions;
using Funko_Api.Data;
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
        var funko = await CrearFunkoAsync("Funko de prueba", 2);

        var creado = await _repository.CreateAsync(funko);

        creado.Id.Should().BePositive();
        creado.Name.Should().Be("Funko de prueba");
        creado.Categoria.Id.Should().Be(2);
        creado.IsDeleted.Should().BeFalse();
        creado.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        creado.UpdatedAt.Should().Be(creado.CreatedAt);
    }

    [Test]
    public async Task GetAllAsync_SinFiltros_RetornaFunkosConCategoria() {
        var funkos = await _repository.GetAllAsync();

        funkos.Should().HaveCount(5);
        funkos.Should().OnlyContain(funko => funko.Categoria != null);
    }

    [Test]
    public async Task SearchByNameAsync_SinDistinguirMayusculas_RetornaCoincidencias() {
        var funkos = await _repository.SearchByNameAsync("NARUTO");

        funkos.Should().ContainSingle()
            .Which.Name.Should().Be("Naruto Uzumaki");
        funkos.Should().OnlyContain(funko => funko.Categoria != null);
    }

    [Test]
    public async Task SearchByNameAsync_SinCoincidencias_RetornaListaVacia() {
        var funkos = await _repository.SearchByNameAsync("no existe");

        funkos.Should().BeEmpty();
    }

    [Test]
    public async Task SearchByNameAsync_IgnoraFunkosBorrados() {
        await _repository.DeleteAsync(1);

        var funkos = await _repository.SearchByNameAsync("naruto");

        funkos.Should().BeEmpty();
    }

    [Test]
    public async Task GetByIdAsync_CuandoExiste_RetornaFunko() {
        var funko = await _repository.GetByIdAsync(1);

        funko.Should().NotBeNull();
        funko!.Name.Should().Be("Naruto Uzumaki");
        funko.Categoria.Name.Should().Be("Anime");
    }

    [Test]
    public async Task GetByIdAsync_CuandoNoExiste_RetornaNull() {
        var funko = await _repository.GetByIdAsync(999);

        funko.Should().BeNull();
    }

    [Test]
    public async Task UpdateAsync_ConDatosValidos_ActualizaFunkoExistente() {
        var funko = await CrearFunkoAsync("Funko actualizado", 2);

        var actualizado = await _repository.UpdateAsync(1, funko);

        actualizado.Should().NotBeNull();
        actualizado!.Id.Should().Be(1);
        actualizado.Name.Should().Be("Funko actualizado");
        actualizado.Price.Should().Be(19.99m);
        actualizado.Stock.Should().Be(3);
        actualizado.Categoria.Id.Should().Be(2);
        actualizado.UpdatedAt.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public async Task UpdateAsync_CuandoNoExiste_RetornaNull() {
        var actualizado = await _repository.UpdateAsync(999, await CrearFunkoAsync("Funko", 1));

        actualizado.Should().BeNull();
    }

    [Test]
    public async Task UpdateAsync_CuandoEstaBorrado_RetornaNull() {
        await _repository.DeleteAsync(1);

        var actualizado = await _repository.UpdateAsync(1, await CrearFunkoAsync("Funko", 1));

        actualizado.Should().BeNull();
    }

    [Test]
    public async Task DeleteAsync_CuandoExiste_BorraLogicamente() {
        var deleted = await _repository.DeleteAsync(1);

        deleted.Should().BeTrue();
        (await _repository.GetByIdAsync(1)).Should().BeNull();
        (await _context.Funkos.IgnoreQueryFilters()
            .AnyAsync(funko => funko.Id == 1 && funko.IsDeleted)).Should().BeTrue();
    }

    [Test]
    public async Task DeleteAsync_CuandoNoExiste_RetornaFalse() {
        var deleted = await _repository.DeleteAsync(999);

        deleted.Should().BeFalse();
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
