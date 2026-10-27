using System;

namespace Funko_Api.Dtos;

/// <summary>
/// DTO de salida con los datos de un funko.
/// </summary>
/// <param name="Id">Identificador del funko.</param>
/// <param name="Name">Nombre del funko.</param>
/// <param name="Price">Precio de venta.</param>
/// <param name="Stock">Unidades disponibles.</param>
/// <param name="CategoriaName">Nombre de la categoría.</param>
/// <param name="CreatedAt">Fecha de creación en UTC.</param>
/// <param name="UpdatedAt">Fecha de la última modificación en UTC.</param>
/// <param name="IsDeleted">Indica si está borrado lógicamente.</param>
public record FunkoDto(
    long Id,
    string Name,
    decimal Price,
    int Stock,
    string CategoriaName,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    bool IsDeleted
);
