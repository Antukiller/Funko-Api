using CSharpFunctionalExtensions;
using FluentAssertions;
using Funko_Api.Dto;
using Funko_Api.Errors;
using Funko_Api.Models;
using Funko_Api.Repositories;
using Funko_Api.Repositories.Categoria;
using Funko_Api.Services;
using Funko_Api.Validator;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Funko_Api.Test.Services;

[TestFixture]
public class FunkoServiceTests {
    private Mock<IFunkoRepository> _funkoRepositoryMock = null!;
    private Mock<ICategoriaRepository> _categoriaRepositoryMock = null!;
    private FunkoService _service = null!;

    [SetUp]
    public void SetUp() {
        _funkoRepositoryMock = new Mock<IFunkoRepository>();
        _categoriaRepositoryMock = new Mock<ICategoriaRepository>();
        _categoriaRepositoryMock
            .Setup(repository => repository.GetByNameAsync(It.IsAny<string>()))
            .ReturnsAsync(Result.Success<Categoria, DomainError>(CrearCategoria()));
        _service = new FunkoService(
            _funkoRepositoryMock.Object,
            _categoriaRepositoryMock.Object,
            new FunkoRequestValidator(),
            NullLogger<FunkoService>.Instance);
    }

    [Test]
    public async Task GetAllFunkosAsync_ConResultados_RetornaDtos() {
        _funkoRepositoryMock.Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<Funko> { CrearFunko() });

        var result = await _service.GetAllFunkosAsync();

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new {
                Name = "Batman Classic",
                CategoriaName = "DC Comics"
            });
        _funkoRepositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_CuandoExiste_RetornaDto() {
        _funkoRepositoryMock.Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(Result.Success<Funko, DomainError>(CrearFunko()));

        var result = await _service.GetByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new {
            Id = 1L,
            Name = "Batman Classic",
            CategoriaName = "DC Comics"
        });
        _funkoRepositoryMock.Verify(repository => repository.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_CuandoNoExiste_PropagaNotFound() {
        var error = NotFound.FromId(8, "Funko");
        _funkoRepositoryMock.Setup(repository => repository.GetByIdAsync(8))
            .ReturnsAsync(Result.Failure<Funko, DomainError>(error));

        var result = await _service.GetByIdAsync(8);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>()
            .Which.Message.Should().Contain("8");
    }

    [Test]
    public async Task CreateAsync_ConDatosValidos_GuardaYRetornaDto() {
        var request = CrearCreateRequest();
        _funkoRepositoryMock.Setup(repository => repository.CreateAsync(It.IsAny<Funko>()))
            .ReturnsAsync((Funko funko) => {
                funko.Id = 11;
                return Result.Success<Funko, DomainError>(funko);
            });

        var result = await _service.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(11);
        result.Value.Name.Should().Be(request.Name);
        result.Value.CategoriaName.Should().Be("DC Comics");
        _funkoRepositoryMock.Verify(repository => repository.CreateAsync(
            It.Is<Funko>(funko => funko.Name == request.Name &&
                                  funko.Price == request.Price &&
                                  funko.Stock == request.Stock &&
                                  funko.Categoria.Name == "DC Comics")), Times.Once);
        _categoriaRepositoryMock.Verify(
            repository => repository.GetByNameAsync("DC Comics"), Times.Once);
    }

    [Test]
    public async Task CreateAsync_ConPeticionNula_RetornaValidationErrorSinConsultarCategoria() {
        var result = await _service.CreateAsync(null!);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
        _categoriaRepositoryMock.Verify(
            repository => repository.GetByNameAsync(It.IsAny<string>()), Times.Never);
        _funkoRepositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<Funko>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_CuandoCategoriaNoExiste_PropagaErrorSinGuardar() {
        var error = NotFound.FromId(99, "Categoría");
        _categoriaRepositoryMock.Setup(repository => repository.GetByNameAsync("DC Comics"))
            .ReturnsAsync(Result.Failure<Categoria, DomainError>(error));

        var result = await _service.CreateAsync(CrearCreateRequest());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
        _funkoRepositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<Funko>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_CuandoValidacionFalla_RetornaValidationErrorSinGuardar() {
        var request = CrearCreateRequest() with { Name = "" };

        var result = await _service.CreateAsync(request);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>()
            .Which.Errors.Should().ContainKey("Name");
        _funkoRepositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<Funko>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_CuandoRepositorioFalla_PropagaError() {
        var error = new ValidationError("No se pudo guardar", []);
        _funkoRepositoryMock.Setup(repository => repository.CreateAsync(It.IsAny<Funko>()))
            .ReturnsAsync(Result.Failure<Funko, DomainError>(error));

        var result = await _service.CreateAsync(CrearCreateRequest());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
        _funkoRepositoryMock.Verify(
            repository => repository.CreateAsync(It.IsAny<Funko>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_ConDatosValidos_ActualizaYRetornaDto() {
        _funkoRepositoryMock.Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(Result.Success<Funko, DomainError>(CrearFunko()));
        var request = CrearUpdateRequest();
        _funkoRepositoryMock.Setup(repository => repository.UpdateAsync(1, It.IsAny<Funko>()))
            .ReturnsAsync((long _, Funko funko) =>
                Result.Success<Funko, DomainError>(funko));

        var result = await _service.UpdateAsync(1, request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Price.Should().Be(request.Price);
        result.Value.Stock.Should().Be(request.Stock);
        result.Value.CategoriaName.Should().Be("DC Comics");
        _funkoRepositoryMock.Verify(repository => repository.GetByIdAsync(1), Times.Once);
        _funkoRepositoryMock.Verify(repository => repository.UpdateAsync(1,
            It.Is<Funko>(funko => funko.Name == request.Name &&
                                  funko.Price == request.Price &&
                                  funko.Stock == request.Stock)), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_ConPeticionNula_RetornaValidationErrorSinConsultarRepositorio() {
        var result = await _service.UpdateAsync(1, null!);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>();
        _funkoRepositoryMock.Verify(
            repository => repository.GetByIdAsync(It.IsAny<long>()), Times.Never);
        _funkoRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<long>(), It.IsAny<Funko>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_CuandoFunkoNoExiste_PropagaNotFoundSinActualizar() {
        var error = NotFound.FromId(8, "Funko");
        _funkoRepositoryMock.Setup(repository => repository.GetByIdAsync(8))
            .ReturnsAsync(Result.Failure<Funko, DomainError>(error));

        var result = await _service.UpdateAsync(8, CrearUpdateRequest());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
        _categoriaRepositoryMock.Verify(
            repository => repository.GetByNameAsync(It.IsAny<string>()), Times.Never);
        _funkoRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<long>(), It.IsAny<Funko>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_CuandoValidacionFalla_RetornaValidationErrorSinActualizar() {
        _funkoRepositoryMock.Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(Result.Success<Funko, DomainError>(CrearFunko()));
        var request = CrearUpdateRequest() with { Stock = -1 };

        var result = await _service.UpdateAsync(1, request);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ValidationError>()
            .Which.Errors.Should().ContainKey("Stock");
        _funkoRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<long>(), It.IsAny<Funko>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_CuandoRepositorioFalla_PropagaError() {
        _funkoRepositoryMock.Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(Result.Success<Funko, DomainError>(CrearFunko()));
        var error = new ValidationError("No se pudo actualizar", []);
        _funkoRepositoryMock.Setup(repository => repository.UpdateAsync(1, It.IsAny<Funko>()))
            .ReturnsAsync(Result.Failure<Funko, DomainError>(error));

        var result = await _service.UpdateAsync(1, CrearUpdateRequest());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
        _funkoRepositoryMock.Verify(
            repository => repository.UpdateAsync(1, It.IsAny<Funko>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_CuandoExiste_RetornaExito() {
        _funkoRepositoryMock.Setup(repository => repository.DeleteAsync(1))
            .ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _funkoRepositoryMock.Verify(repository => repository.DeleteAsync(1), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_CuandoNoExiste_RetornaNotFound() {
        _funkoRepositoryMock.Setup(repository => repository.DeleteAsync(8))
            .ReturnsAsync(false);

        var result = await _service.DeleteAsync(8);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<NotFound>()
            .Which.Message.Should().Contain("8");
    }

    [Test]
    public async Task GetAllCategoriesAsync_ConResultados_RetornaDtos() {
        _categoriaRepositoryMock.Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(new List<Categoria> { CrearCategoria() });

        var result = await _service.GetAllCategoriesAsync();

        result.Should().ContainSingle()
            .Which.Should().Be(new CategoriaDto(1, "DC Comics"));
        _categoriaRepositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetCategoryByIdAsync_CuandoExiste_RetornaDto() {
        _categoriaRepositoryMock.Setup(repository => repository.GetByIdAsync(1))
            .ReturnsAsync(Result.Success<Categoria, DomainError>(CrearCategoria()));

        var result = await _service.GetCategoryByIdAsync(1);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(new CategoriaDto(1, "DC Comics"));
    }

    [Test]
    public async Task GetCategoryByIdAsync_CuandoNoExiste_PropagaNotFound() {
        var error = NotFound.FromId(8, "Categoría");
        _categoriaRepositoryMock.Setup(repository => repository.GetByIdAsync(8))
            .ReturnsAsync(Result.Failure<Categoria, DomainError>(error));

        var result = await _service.GetCategoryByIdAsync(8);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeSameAs(error);
    }

    private static Funko CrearFunko() => new() {
        Id = 1,
        Name = "Batman Classic",
        Price = 19.99m,
        Stock = 5,
        Categoria = CrearCategoria()
    };

    private static Categoria CrearCategoria() => new() {
        Id = 1,
        Name = "DC Comics"
    };

    private static CreateFunkoDto CrearCreateRequest() => new() {
        Name = "Batman Classic",
        Price = 19.99m,
        Stock = 5,
        CategoriaName = "DC Comics"
    };

    private static UpdateFunkoDto CrearUpdateRequest() => new() {
        Name = "Batman actualizado",
        Price = 24.99m,
        Stock = 3,
        CategoriaName = "DC Comics"
    };
}
