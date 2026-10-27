using Funko_Api.Dto;
using Funko_Api.Errors;
using Funko_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Funko_Api.Controller;

/// <summary>
/// API controller para gestionar Funkos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FunkosController(IFunkoService service) : ControllerBase {
    /// <summary>
    /// Obtiene todos los Funkos activos.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FunkoDto>>> GetAll() =>
        Ok(await service.GetAllFunkosAsync());

    /// <summary>
    /// Busca Funkos cuyo nombre contenga el texto indicado.
    /// </summary>
    [HttpGet("buscar")]
    public async Task<IActionResult> SearchByName([FromQuery] string nombre) {
        if (string.IsNullOrWhiteSpace(nombre))
            return ValidationError.Create("El nombre de búsqueda no puede estar vacío.").ToHttpResult();

        return Ok(await service.SearchByNameAsync(nombre));
    }

    /// <summary>
    /// Obtiene un Funko por su ID.
    /// </summary>
    [HttpGet("{id:long}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById(long id) =>
        (await service.GetByIdAsync(id)).ToHttpResult();

    /// <summary>
    /// Crea un nuevo Funko.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFunkoDto request) {
        var result = await service.CreateAsync(request);
        if (result.IsFailure)
            return result.Error.ToHttpResult();

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Actualiza un Funko existente.
    /// </summary>
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateFunkoDto request) =>
        (await service.UpdateAsync(id, request)).ToHttpResult();

    /// <summary>
    /// Actualiza solo el precio de un Funko.
    /// </summary>
    [HttpPatch("{id:long}/precio")]
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
    [HttpPatch("{id:long}/stock")]
    public async Task<IActionResult> PatchStock(long id, [FromBody] int cantidad) =>
        (await service.AdjustStockAsync(id, cantidad)).ToHttpResult();

    /// <summary>
    /// Elimina un Funko (soft delete).
    /// </summary>
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id) {
        var result = await service.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : result.Error.ToHttpResult();
    }
}
