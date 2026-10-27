using Funko_Api.Dtos;
using Funko_Api.Models;

namespace Funko_Api.Mappers;

/// <summary>
/// Conversiones entre las entidades de dominio y sus DTOs.
/// </summary>
public static class FunkoMapper {
    /// <summary>
    /// Convierte un funko de dominio en su DTO de salida.
    /// </summary>
    /// <param name="funko">Entidad a convertir.</param>
    /// <returns>DTO con los datos del funko.</returns>
    public static FunkoDto ToDto(this Funko funko) => new(
        funko.Id,
        funko.Name,
        funko.Price,
        funko.Stock,
        funko.Categoria.Name,
        funko.CreatedAt,
        funko.UpdatedAt,
        funko.IsDeleted);

    /// <summary>
    /// Convierte un DTO de creación en una entidad de dominio.
    /// </summary>
    /// <param name="dto">DTO de entrada.</param>
    /// <param name="categoria">Categoría ya resuelta.</param>
    /// <returns>Entidad lista para persistir.</returns>
    public static Funko ToModel(this CreateFunkoDto dto, Categoria categoria) => new() {
        Name = dto.Name,
        Price = dto.Price,
        Stock = dto.Stock,
        Categoria = categoria
    };

    /// <summary>
    /// Convierte un DTO de actualización en una entidad de dominio.
    /// </summary>
    /// <param name="dto">DTO de entrada.</param>
    /// <param name="categoria">Categoría ya resuelta.</param>
    /// <returns>Entidad con los valores actualizados.</returns>
    public static Funko ToModel(this UpdateFunkoDto dto, Categoria categoria) => new() {
        Name = dto.Name,
        Price = dto.Price,
        Stock = dto.Stock,
        Categoria = categoria
    };
}
