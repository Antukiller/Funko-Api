using CSharpFunctionalExtensions;
using FluentAssertions;
using Funko_Api.Dtos;
using Funko_Api.Errors;
using Funko_Api.Models;
using Funko_Api.Repositories.Categoria;
using Funko_Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Funko_Api.Test.Services;

[TestFixture]
public class CategoriaServiceTests {
    private Mock<ICategoriaRepository> _categoriaRepositoryMock = null!;
    private CategoriaService _service = null!;

    [SetUp]
    public void SetUp() {
        _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
        _service = new CategoriaService(
            _categoriaRepositoryMock.Object,
            NullLogger<CategoriaService>.Instance);
    }

    [Test]
    public async Task GetAllAsync_ConCategorias_RetornaDtos() {
        _categoriaRepositoryMock.Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<Categoria> { CrearCategoria() });

        var result = await _service.GetAllAsync();

        result.Should().ContainSingle()
            .Which.Should().Be(new CategoriaDto(1, "DC Comics"));
        _categoriaRepositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_SinCategorias_RetornaListaVacia() {
        _categoriaRepositoryMock.Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<Categoria>());

        var result = await _service.GetAllAsync();

        result.Should().BeEmpty();
        _categoriaRepositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_CuandoExiste_RetornaDto() {
        _categoriaRepositoryMock.Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(CrearCategoria());

        var result = await _service.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(new CategoriaDto(1, "DC Comics"));
        _categoriaRepositoryMock.Verify(repository => repository.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_CuandoNoExiste_RetornaNotFound() {
        _categoriaRepositoryMock.Setup(repository => repository.GetByIdAsync(8))
            .ReturnsAsync((Categoria?)null);

        var result = await _service.GetByIdAsync(8);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>()
            .Which.Message.Should().Contain("8");
    }

    private static Categoria CrearCategoria() => new() {
        Id = 1,
        Name = "DC Comics"
    };
}
