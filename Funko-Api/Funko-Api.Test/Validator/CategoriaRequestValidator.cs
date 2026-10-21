using FluentAssertions;
using Funko_Api.Models;
using Funko_Api.Validator;

namespace Funko_Api.Test.Validator;

public class CategoriaRequestValidatorTests {
    private readonly CategoriaRequestValidator _validator = new();

    [Test]
    public void Validate_ConNombreVacio_TieneErrorDeNombre() {
        var categoria = new Categoria { Name = "" };

        var result = _validator.Validate(categoria);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == nameof(Categoria.Name) &&
            error.ErrorMessage == "Name is required.");
    }

    [Test]
    public void Validate_ConNombreMayorA200Caracteres_TieneErrorDeNombre() {
        var categoria = new Categoria { Name = new string('A', 201) };

        var result = _validator.Validate(categoria);

        result.Errors.Should().ContainSingle(error =>
            error.PropertyName == nameof(Categoria.Name) &&
            error.ErrorMessage == "Name cannot exceed 200 characters.");
    }

    [Test]
    public void Validate_ConNombreValido_NoTieneErrores() {
        var categoria = new Categoria { Name = "Anime" };

        var result = _validator.Validate(categoria);

        result.Errors.Should().BeEmpty();
    }
}
