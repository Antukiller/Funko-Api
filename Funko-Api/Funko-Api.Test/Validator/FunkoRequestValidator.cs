using FluentValidation.TestHelper;
using Funko_Api.Models;
using Funko_Api.Validator;

namespace Funko_Api.Test.Validator;

public class FunkoRequestValidatorTests {
    private readonly FunkoRequestValidator _validator = new();

    [Test]
    public void Validate_ConNombreVacio_TieneErrorDeNombre() {
        var funko = CreateValidFunko();
        funko.Name = " ";

        var result = _validator.TestValidate(funko);

        result.ShouldHaveValidationErrorFor(item => item.Name)
            .WithErrorMessage("Name is required.");
    }

    [Test]
    public void Validate_ConNombreMayorA200Caracteres_TieneErrorDeNombre() {
        var funko = CreateValidFunko();
        funko.Name = new string('A', 201);

        var result = _validator.TestValidate(funko);

        result.ShouldHaveValidationErrorFor(item => item.Name)
            .WithErrorMessage("Name cannot exceed 200 characters.");
    }

    [Test]
    public void Validate_ConNombreValido_NoTieneErrorDeNombre() {
        var result = _validator.TestValidate(CreateValidFunko());

        result.ShouldNotHaveValidationErrorFor(item => item.Name);
    }

    [Test]
    public void Validate_ConFunkoNuevo_TieneErrorDeNombrePorDefectoVacio() {
        var funko = new Funko {
            Categoria = new Categoria { Name = "DC Comics" }
        };

        var result = _validator.TestValidate(funko);

        result.ShouldHaveValidationErrorFor(item => item.Name)
            .WithErrorMessage("Name is required.");
    }

    [TestCase(0)]
    [TestCase(-0.01)]
    public void Validate_ConPrecioNoPositivo_TieneErrorDePrecio(decimal price) {
        var funko = CreateValidFunko();
        funko.Price = price;

        var result = _validator.TestValidate(funko);

        result.ShouldHaveValidationErrorFor(item => item.Price)
            .WithErrorMessage("Price must be greater than 0.");
    }

    [Test]
    public void Validate_ConPrecioPositivo_NoTieneErrorDePrecio() {
        var funko = CreateValidFunko();
        funko.Price = 0.01m;

        var result = _validator.TestValidate(funko);

        result.ShouldNotHaveValidationErrorFor(item => item.Price);
    }

    [Test]
    public void Validate_ConStockNegativo_TieneErrorDeStock() {
        var funko = CreateValidFunko();
        funko.Stock = -1;

        var result = _validator.TestValidate(funko);

        result.ShouldHaveValidationErrorFor(item => item.Stock)
            .WithErrorMessage("Stock must be greater than or equal to 0.");
    }

    [TestCase(0)]
    [TestCase(25)]
    public void Validate_ConStockNoNegativo_NoTieneErrorDeStock(int stock) {
        var funko = CreateValidFunko();
        funko.Stock = stock;

        var result = _validator.TestValidate(funko);

        result.ShouldNotHaveValidationErrorFor(item => item.Stock);
    }

    [Test]
    public void Validate_ConNombreDeCategoriaVacio_TieneErrorEnCategoria() {
        var funko = CreateValidFunko();
        funko.Categoria.Name = "";

        var result = _validator.TestValidate(funko);

        result.ShouldHaveValidationErrorFor(item => item.Categoria.Name)
            .WithErrorMessage("Name is required.");
    }

    [Test]
    public void Validate_ConNombreDeCategoriaMayorA200Caracteres_TieneErrorEnCategoria() {
        var funko = CreateValidFunko();
        funko.Categoria.Name = new string('A', 201);

        var result = _validator.TestValidate(funko);

        result.ShouldHaveValidationErrorFor(item => item.Categoria.Name)
            .WithErrorMessage("Name cannot exceed 200 characters.");
    }

    [Test]
    public void Validate_ConDatosValidos_NoTieneErrores() {
        var result = _validator.TestValidate(CreateValidFunko());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_ConDatosInvalidos_TieneErroresEnLasPropiedadesInvalidas() {
        var funko = CreateValidFunko();
        funko.Name = "";
        funko.Price = 0;
        funko.Stock = -1;
        funko.Categoria.Name = "";

        var result = _validator.TestValidate(funko);

        result.ShouldHaveValidationErrorFor(item => item.Name);
        result.ShouldHaveValidationErrorFor(item => item.Price);
        result.ShouldHaveValidationErrorFor(item => item.Stock);
        result.ShouldHaveValidationErrorFor(item => item.Categoria.Name);
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
