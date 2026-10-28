using Funko_Api.Dtos;
using Funko_Api.Errors;
using Funko_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Funko_Api.Controllers;

/// <summary>
/// API controller para gestionar Funkos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FunkosController(IFunkoService service) : ControllerBase {
    /// <summary>
    /// Obtiene todos los Funkos activos.
    /// </summary>
    /// <returns>Lista de funkos con sus categorías.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<FunkoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FunkoDto>>> GetAll() =>
        Ok(await service.GetAllFunkosAsync());

    /// <summary>
    /// Busca Funkos cuyo nombre contenga el texto indicado.
    /// </summary>
    /// <param name="nombre">Texto a buscar.</param>
    /// <returns>Lista de funkos que coinciden.</returns>
    [HttpGet("buscar")]
    [ProducesResponseType(typeof(IReadOnlyList<FunkoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchByName([FromQuery] string nombre) {
        if (string.IsNullOrWhiteSpace(nombre))
            return ValidationError.Create("El nombre de búsqueda no puede estar vacío.").ToHttpResult();

        return Ok(await service.SearchByNameAsync(nombre));
    }

    /// <summary>
    /// Obtiene un Funko por su ID.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <returns>El funko encontrado.</returns>
    [HttpGet("{id:long}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(FunkoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id) =>
        (await service.GetByIdAsync(id)).ToHttpResult();

    /// <summary>
    /// Crea un nuevo Funko.
    /// </summary>
    /// <param name="request">Datos del funko.</param>
    /// <returns>El funko creado con su ubicación.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(FunkoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateFunkoDto request) {
        var result = await service.CreateAsync(request);
        if (result.IsFailure)
            return result.Error.ToHttpResult();

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Actualiza un Funko existente.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <param name="request">Nuevos datos.</param>
    /// <returns>El funko actualizado.</returns>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(FunkoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateFunkoDto request) =>
        (await service.UpdateAsync(id, request)).ToHttpResult();

    /// <summary>
    /// Actualiza solo el precio de un Funko.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <param name="precio">Nuevo precio.</param>
    /// <returns>El funko actualizado.</returns>
    [HttpPatch("{id:long}/precio")]
    [ProducesResponseType(typeof(FunkoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PatchPrice(long id, [FromBody] decimal precio) {
        var existente = await service.GetByIdAsync(id);
        if (existente.IsFailure)
            return existente.Error.ToHttpResult();

        var funko = existente.Value;
        var result = await service.UpdateAsync(id, new UpdateFunkoDto {
            Name = funko.Name,
            Price = precio,
            Stock = funko.Stock,
            CategoriaName = funko.CategoriaName
        });
        return result.ToHttpResult();
    }

    /// <summary>
    /// Ajusta el stock de un Funko: una cantidad positiva repone y una negativa descuenta.
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <param name="cantidad">Unidades a sumar (negativas para descontar).</param>
    /// <returns>El funko con el nuevo stock.</returns>
    [HttpPatch("{id:long}/stock")]
    [ProducesResponseType(typeof(FunkoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PatchStock(long id, [FromBody] int cantidad) =>
        (await service.AdjustStockAsync(id, cantidad)).ToHttpResult();

    /// <summary>
    /// Elimina un Funko (soft delete).
    /// </summary>
    /// <param name="id">Identificador del funko.</param>
    /// <returns>Respuesta vacía con estado 204.</returns>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id) {
        var result = await service.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : result.Error.ToHttpResult();
    }
}
