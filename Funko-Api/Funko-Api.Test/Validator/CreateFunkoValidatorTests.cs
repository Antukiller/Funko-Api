using FluentAssertions;
using Funko_Api.Dto;
using Funko_Api.Validator;

namespace Funko_Api.Test.Validator;

[TestFixture]
public class CreateFunkoValidatorTests {
    [Test]
    public void Validate_ConDatosValidos_NoTieneErrores() {
        var result = new CreateFunkoValidator().Validate(CrearRequest());

        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_ConDatosInvalidos_IndicaPropiedadesInvalidas() {
        var request = CrearRequest() with {
            Name = " ",
            Price = 0,
            Stock = -1,
            CategoriaName = ""
        };

        var result = new CreateFunkoValidator().Validate(request);

        result.Errors.Select(error => error.PropertyName).Should().BeEquivalentTo(
            nameof(CreateFunkoDto.Name),
            nameof(CreateFunkoDto.Price),
            nameof(CreateFunkoDto.Stock),
            nameof(CreateFunkoDto.CategoriaName));
    }

    private static CreateFunkoDto CrearRequest() => new() {
        Name = "Batman Classic",
        Price = 19.99m,
        Stock = 5,
        CategoriaName = "DC Comics"
    };
}
