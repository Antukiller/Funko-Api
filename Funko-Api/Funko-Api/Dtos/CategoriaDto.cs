namespace Funko_Api.Dtos;

/// <summary>
/// DTO de salida con los datos de una categoría.
/// </summary>
/// <param name="Id">Identificador de la categoría.</param>
/// <param name="Name">Nombre de la categoría.</param>
public record CategoriaDto(long Id, string Name);
