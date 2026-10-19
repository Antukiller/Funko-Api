using FluentValidation.TestHelper;
using Funko_Api.Models;
using Funko_Api.Validator;

namespace Funko_Api.Test.Validator;

public class CategoriaRequestValidatorTests {
    private readonly CategoriaRequestValidator _validator = new();

    [Test]
    public void Validate_ConNombreVacio_TieneErrorDeNombre() {
        var categoria = new Categoria { Name = "" };

        var result = _validator.TestValidate(categoria);

        result.ShouldHaveValidationErrorFor(item => item.Name)
            .WithErrorMessage("Name is required.");
    }

    [Test]
    public void Validate_ConNombreMayorA200Caracteres_TieneErrorDeNombre() {
        var categoria = new Categoria { Name = new string('A', 201) };

        var result = _validator.TestValidate(categoria);

        result.ShouldHaveValidationErrorFor(item => item.Name)
            .WithErrorMessage("Name cannot exceed 200 characters.");
    }

    [Test]
    public void Validate_ConNombreValido_NoTieneErrores() {
        var categoria = new Categoria { Name = "Anime" };

        var result = _validator.TestValidate(categoria);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
