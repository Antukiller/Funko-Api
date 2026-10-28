using Funko_Api.Dtos;
using Funko_Api.Errors;
using Funko_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Funko_Api.Controllers;

/// <summary>
/// API controller para consultar categorías.
/// </summary>
[ApiController]
[Route("api/categorias")]
public class CategoriasController(ICategoriaService service) : ControllerBase {
    /// <summary>
    /// Obtiene todas las categorías.
    /// </summary>
    /// <returns>Lista de categorías.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoriaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll() =>
        Ok(await service.GetAllAsync());

    /// <summary>
    /// Obtiene una categoría por su ID.
    /// </summary>
    /// <param name="id">Identificador de la categoría.</param>
    /// <returns>La categoría encontrada.</returns>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CategoriaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoriaDto>> GetById(long id) =>
        (await service.GetByIdAsync(id)).ToHttpResult();
}
