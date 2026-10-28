using FluentAssertions;
using Funko_Api.Dtos;
using Funko_Api.Validators;

namespace Funko_Api.Test.Unit.Validators;

[Category(TestCategories.Unit)]
[TestFixture]
public class UpdateFunkoValidatorTests {
    [Test]
    public void Validate_ConDatosValidos_NoTieneErrores() {
        var result = new UpdateFunkoValidator().Validate(CrearRequest());

        result.Errors.Should().BeEmpty();
    }

    [Test]
    public void Validate_ConDatosInvalidos_IndicaPropiedadesInvalidas() {
        var request = CrearRequest() with {
            Name = new string('A', 201),
            Price = -1,
            Stock = -1,
            CategoriaName = new string('A', 201)
        };

        var result = new UpdateFunkoValidator().Validate(request);

        result.Errors.Select(error => error.PropertyName).Should().BeEquivalentTo(
            nameof(UpdateFunkoDto.Name),
            nameof(UpdateFunkoDto.Price),
            nameof(UpdateFunkoDto.Stock),
            nameof(UpdateFunkoDto.CategoriaName));
    }

    private static UpdateFunkoDto CrearRequest() => new() {
        Name = "Batman Classic",
        Price = 19.99m,
        Stock = 5,
        CategoriaName = "DC Comics"
    };
}
