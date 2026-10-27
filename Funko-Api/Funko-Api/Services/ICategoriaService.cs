using CSharpFunctionalExtensions;
using Funko_Api.Dto;
using Funko_Api.Errors;

namespace Funko_Api.Services;

/// <summary>
/// Servicio de negocio para la gestión de categorías.
/// </summary>
public interface ICategoriaService {
    /// <summary>
    /// Obtiene todas las categorías disponibles.
    /// </summary>
    /// <returns>Lista de categorías como DTO.</returns>
    Task<IReadOnlyList<CategoriaDto>> GetAllAsync();

    /// <summary>
    /// Obtiene una categoría por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la categoría.</param>
    /// <returns>La categoría encontrada o un error de dominio si no existe.</returns>
    Task<Result<CategoriaDto, DomainError>> GetByIdAsync(long id);
}
