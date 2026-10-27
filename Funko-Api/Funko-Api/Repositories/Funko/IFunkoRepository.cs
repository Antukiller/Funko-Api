using Funko_Api.Models;

namespace Funko_Api.Repositories;

/// <summary>
/// Contrato de acceso a datos de los funkos.
/// </summary>
public interface IFunkoRepository {
    /// <summary>
    /// Obtiene todos los funkos activos con su categoría.
    /// </summary>
    /// <returns>Lista de funkos.</returns>
    Task<IReadOnlyList<Funko>> GetAllAsync();

    /// <summary>
    /// Busca funkos cuyo nombre contenga el texto indicado.
    /// </summary>
    /// <param name="nombre">Texto a buscar.</param>
    /// <returns>Lista de funkos que coinciden.</returns>
    Task<IReadOnlyList<Funko>> SearchByNameAsync(string nombre);

    /// <summary>
    /// Comprueba si ya existe un funko con ese nombre.
    /// </summary>
    /// <param name="name">Nombre a comprobar.</param>
    /// <param name="excludeId">Identificador a excluir (en actualizaciones).</param>
    /// <returns><c>true</c> si el nombre ya existe.</returns>
    Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

    /// <summary>
    /// Obtiene un funko por su identificador.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <returns>El funko o <c>null</c> si no existe.</returns>
    Task<Funko?> GetByIdAsync(long id);

    /// <summary>
    /// Persiste un funko nuevo.
    /// </summary>
    /// <param name="funko">Entidad a crear.</param>
    /// <returns>El funko creado con su identificador.</returns>
    Task<Funko> CreateAsync(Funko funko);

    /// <summary>
    /// Actualiza los datos de un funko existente.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <param name="funko">Nuevos valores.</param>
    /// <returns>El funko actualizado o <c>null</c> si no existe.</returns>
    Task<Funko?> UpdateAsync(long id, Funko funko);

    /// <summary>
    /// Actualiza únicamente el stock de un funko.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <param name="stock">Nuevo stock.</param>
    /// <returns>El funko actualizado o <c>null</c> si no existe.</returns>
    Task<Funko?> UpdateStockAsync(long id, int stock);

    /// <summary>
    /// Marca un funko como borrado lógicamente.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <returns><c>true</c> si se ha borrado.</returns>
    Task<bool> DeleteAsync(long id);
}
