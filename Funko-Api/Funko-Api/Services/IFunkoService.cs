using CSharpFunctionalExtensions;
using Funko_Api.Dtos;
using Funko_Api.Errors;

namespace Funko_Api.Services;

/// <summary>
/// Servicio de negocio para la gestión de funkos.
/// </summary>
public interface IFunkoService {
    /// <summary>
    /// Obtiene todos los funkos activos.
    /// </summary>
    /// <returns>Lista de funkos como DTO.</returns>
    Task<IReadOnlyList<FunkoDto>> GetAllFunkosAsync();

    /// <summary>
    /// Busca funkos por nombre.
    /// </summary>
    /// <param name="nombre">Texto a buscar.</param>
    /// <returns>Lista de funkos que coinciden.</returns>
    Task<IReadOnlyList<FunkoDto>> SearchByNameAsync(string nombre);

    /// <summary>
    /// Obtiene un funko por su identificador.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <returns>El funko o un error de dominio si no existe.</returns>
    Task<Result<FunkoDto, DomainError>> GetByIdAsync(long id);

    /// <summary>
    /// Crea un funko validando el DTO y el nombre duplicado.
    /// </summary>
    /// <param name="request">Datos del funko.</param>
    /// <returns>El funko creado o el error de dominio correspondiente.</returns>
    Task<Result<FunkoDto, DomainError>> CreateAsync(CreateFunkoDto request);

    /// <summary>
    /// Actualiza un funko existente.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <param name="request">Nuevos datos.</param>
    /// <returns>El funko actualizado o el error de dominio correspondiente.</returns>
    Task<Result<FunkoDto, DomainError>> UpdateAsync(long id, UpdateFunkoDto request);

    /// <summary>
    /// Ajusta el stock de un funko sumando la cantidad indicada.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <param name="cantidad">Unidades a sumar (negativas para descontar).</param>
    /// <returns>El funko actualizado o el error de dominio correspondiente.</returns>
    Task<Result<FunkoDto, DomainError>> AdjustStockAsync(long id, int cantidad);

    /// <summary>
    /// Elimina un funko con borrado lógico.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <returns><c>true</c> si se ha borrado o el error de dominio correspondiente.</returns>
    Task<Result<bool, DomainError>> DeleteAsync(long id);
}
