using FluentAssertions;
using Funko_Api.Models;
using Funko_Api.Validator;

namespace Funko_Api.Test.Validator;

public class FunkoRequestValidatorTests {
    private readonly FunkoRequestValidator _validator = new();

    [Test]
    public void Validate_ConNombreVacio_TieneErrorDeNombre() {
        var funko = CreateValidFunko();
        funko.Name = " ";

        var result = _validator.Validate(funko);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == nameof(Funko.Name) &&
            error.ErrorMessage == "Name is required.");
    }

    [Test]
    public void Validate_ConNombreMayorA200Caracteres_TieneErrorDeNombre() {
        var funko = CreateValidFunko();
        funko.Name = new string('A', 201);

        var result = _validator.Validate(funko);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == nameof(Funko.Name) &&
            error.ErrorMessage == "Name cannot exceed 200 characters.");
    }

    [Test]
    public void Validate_ConNombreValido_NoTieneErrorDeNombre() {
        var result = _validator.Validate(CreateValidFunko());

        result.Errors.Should().NotContain(error => error.PropertyName == nameof(Funko.Name));
    }

    [Test]
    public void Validate_ConFunkoNuevo_TieneErrorDeNombrePorDefectoVacio() {
        var funko = new Funko {
            Categoria = new Categoria { Name = "DC Comics" }
        };

        var result = _validator.Validate(funko);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == nameof(Funko.Name) &&
            error.ErrorMessage == "Name is required.");
    }

    [TestCase(0)]
    [TestCase(-0.01)]
    public void Validate_ConPrecioNoPositivo_TieneErrorDePrecio(decimal price) {
        var funko = CreateValidFunko();
        funko.Price = price;

        var result = _validator.Validate(funko);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == nameof(Funko.Price) &&
            error.ErrorMessage == "Price must be greater than 0.");
    }

    [Test]
    public void Validate_ConPrecioPositivo_NoTieneErrorDePrecio() {
        var funko = CreateValidFunko();
        funko.Price = 0.01m;

        var result = _validator.Validate(funko);

        result.Errors.Should().NotContain(error => error.PropertyName == nameof(Funko.Price));
    }

    [Test]
    public void Validate_ConStockNegativo_TieneErrorDeStock() {
        var funko = CreateValidFunko();
        funko.Stock = -1;

        var result = _validator.Validate(funko);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == nameof(Funko.Stock) &&
            error.ErrorMessage == "Stock must be greater than or equal to 0.");
    }

    [TestCase(0)]
    [TestCase(25)]
    public void Validate_ConStockNoNegativo_NoTieneErrorDeStock(int stock) {
        var funko = CreateValidFunko();
        funko.Stock = stock;

        var result = _validator.Validate(funko);

        result.Errors.Should().NotContain(error => error.PropertyName == nameof(Funko.Stock));
    }

    [Test]
    public void Validate_ConNombreDeCategoriaVacio_TieneErrorEnCategoria() {
        var funko = CreateValidFunko();
        funko.Categoria.Name = "";

        var result = _validator.Validate(funko);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == "Categoria.Name" &&
            error.ErrorMessage == "Category name is required.");
    }

    [Test]
    public void Validate_ConNombreDeCategoriaMayorA200Caracteres_TieneErrorEnCategoria() {
        var funko = CreateValidFunko();
        funko.Categoria.Name = new string('A', 201);

        var result = _validator.Validate(funko);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == "Categoria.Name" &&
            error.ErrorMessage == "Category name cannot exceed 200 characters.");
    }

    [Test]
    public void Validate_ConDatosValidos_NoTieneErrores() {
        var result = _validator.Validate(CreateValidFunko());

        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_ConDatosInvalidos_TieneErroresEnLasPropiedadesInvalidas() {
        var funko = CreateValidFunko();
        funko.Name = "";
        funko.Price = 0;
        funko.Stock = -1;
        funko.Categoria.Name = "";

        var result = _validator.Validate(funko);

        result.Errors.Select(error => error.PropertyName).Should().BeEquivalentTo(
            nameof(Funko.Name),
            nameof(Funko.Price),
            nameof(Funko.Stock),
            "Categoria.Name");
    }

    [Test]
    public void Validate_SinCategoria_TieneErrorDeCategoria() {
        var funko = CreateValidFunko();
        funko.Categoria = null!;

        var result = _validator.Validate(funko);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == nameof(Funko.Categoria) &&
            error.ErrorMessage == "Category is required.");
    }

    private static Funko CreateValidFunko() {
        return new Funko {
            Id = 1,
            Name = "Batman Classic",
            Price = 19.99m,
            Stock = 5,
            Categoria = new Categoria { Name = "DC Comics" }
        };
    }
}
