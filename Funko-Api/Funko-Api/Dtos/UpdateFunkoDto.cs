using System.ComponentModel.DataAnnotations;

namespace Funko_Api.Dtos;

/// <summary>
/// DTO de entrada para actualizar un funko.
/// </summary>
public sealed record UpdateFunkoDto {
    /// <summary>Nombre del funko.</summary>
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre no puede superar los 200 caracteres.")]
    public string Name { get; init; } = string.Empty;

    /// <summary>Precio de venta, mayor que cero.</summary>
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor que 0.")]
    public decimal Price { get; init; }

    /// <summary>Unidades disponibles, no negativas.</summary>
    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; init; }

    /// <summary>Nombre de la categoría existente.</summary>
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre de la categoría no puede superar los 200 caracteres.")]
    public string CategoriaName { get; init; } = string.Empty;
}
