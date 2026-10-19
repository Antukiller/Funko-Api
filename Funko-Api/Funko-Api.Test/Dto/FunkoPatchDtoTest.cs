using FluentAssertions;
using Funko_Api.Dto;

namespace Funko_Api.Test.Dto;

public class FunkoPatchDtoTests {
    [Test]
    public void Constructor_PorDefecto_DejaTodasLasPropiedadesSinCambios() {
        var dto = new FunkoPatchRequestDto();

        dto.Name.Should().BeNull();
        dto.Price.Should().BeNull();
        dto.Stock.Should().BeNull();
        dto.CategoriaName.Should().BeNull();
    }

    [Test]
    public void Inicializador_AsignaTodasLasPropiedades() {
        var dto = new FunkoPatchRequestDto {
            Name = "Batman Classic",
            Price = 19.99m,
            Stock = 5,
            CategoriaName = "DC Comics"
        };

        dto.Name.Should().Be("Batman Classic");
        dto.Price.Should().Be(19.99m);
        dto.Stock.Should().Be(5);
        dto.CategoriaName.Should().Be("DC Comics");
    }

    [Test]
    public void With_CreaUnaCopiaConElNombreActualizado() {
        var original = new FunkoPatchRequestDto {
            Name = "Batman Classic",
            Price = 19.99m,
            Stock = 5,
            CategoriaName = "DC Comics"
        };

        var updated = original with { Name = "Batman 1966" };

        updated.Name.Should().Be("Batman 1966");
        updated.Price.Should().Be(original.Price);
        updated.Stock.Should().Be(original.Stock);
        updated.CategoriaName.Should().Be(original.CategoriaName);
        original.Name.Should().Be("Batman Classic");
    }

    [Test]
    public void Patch_ActualizaSoloElNombre_DejaLasDemasPropiedadesSinCambios() {
        var patch = new FunkoPatchRequestDto { Name = "Batman 1966" };

        patch.Name.Should().Be("Batman 1966");
        patch.Price.Should().BeNull();
        patch.Stock.Should().BeNull();
        patch.CategoriaName.Should().BeNull();
    }

    [Test]
    public void Patch_ActualizaSoloElPrecio_DejaLasDemasPropiedadesSinCambios() {
        var patch = new FunkoPatchRequestDto { Price = 24.99m };

        patch.Price.Should().Be(24.99m);
        patch.Name.Should().BeNull();
        patch.Stock.Should().BeNull();
        patch.CategoriaName.Should().BeNull();
    }

    [Test]
    public void Patch_ActualizaSoloElStock_DejaLasDemasPropiedadesSinCambios() {
        var patch = new FunkoPatchRequestDto { Stock = 12 };

        patch.Stock.Should().Be(12);
        patch.Name.Should().BeNull();
        patch.Price.Should().BeNull();
        patch.CategoriaName.Should().BeNull();
    }

    [Test]
    public void Patch_ActualizaSoloLaCategoria_DejaLasDemasPropiedadesSinCambios() {
        var patch = new FunkoPatchRequestDto { CategoriaName = "Marvel" };

        patch.CategoriaName.Should().Be("Marvel");
        patch.Name.Should().BeNull();
        patch.Price.Should().BeNull();
        patch.Stock.Should().BeNull();
    }
}
