using Funko_Api.Dto;
using Funko_Api.Errors;
using Funko_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Funko_Api.Controller;

/// <summary>
/// API controller para consultar categorías.
/// </summary>
[ApiController]
[Route("api/categorias")]
public class CategoriasController(ICategoriaService service) : ControllerBase {
    /// <summary>
    /// Obtiene todas las categorías.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll() =>
        Ok(await service.GetAllAsync());

    /// <summary>
    /// Obtiene una categoría por su ID.
    /// </summary>
    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id) =>
        (await service.GetByIdAsync(id)).ToHttpResult();
}
