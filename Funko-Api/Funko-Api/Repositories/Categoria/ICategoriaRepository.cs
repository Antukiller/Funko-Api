using CategoriaModel = Funko_Api.Models.Categoria;

namespace Funko_Api.Repositories.Categoria;

/// <summary>
/// Contrato de acceso a datos de las categorías.
/// </summary>
public interface ICategoriaRepository {
    /// <summary>
    /// Obtiene todas las categorías.
    /// </summary>
    /// <returns>Lista de categorías.</returns>
    Task<IReadOnlyList<CategoriaModel>> GetAllAsync();

    /// <summary>
    /// Obtiene una categoría por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la categoría.</param>
    /// <returns>La categoría o <c>null</c> si no existe.</returns>
    Task<CategoriaModel?> GetByIdAsync(long id);

    /// <summary>
    /// Obtiene una categoría por su nombre exacto.
    /// </summary>
    /// <param name="name">Nombre de la categoría.</param>
    /// <returns>La categoría o <c>null</c> si no existe.</returns>
    Task<CategoriaModel?> GetByNameAsync(string name);
}
