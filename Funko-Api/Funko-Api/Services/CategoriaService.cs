using CSharpFunctionalExtensions;
using Funko_Api.Dtos;
using Funko_Api.Errors;
using Funko_Api.Models;
using Funko_Api.Repositories.Categoria;

namespace Funko_Api.Services;

/// <summary>
/// Implementación del servicio de negocio para categorías.
/// </summary>
public class CategoriaService(
    ICategoriaRepository categoriaRepository,
    ILogger<CategoriaService> logger
) : ICategoriaService {
    /// <inheritdoc/>
    public async Task<IReadOnlyList<CategoriaDto>> GetAllAsync() {
        logger.LogInformation("Getting all Categorias");
        var categorias = await categoriaRepository.GetAllAsync();
        return categorias.Select(ToDto).ToList();
    }

    /// <inheritdoc/>
    public async Task<Result<CategoriaDto, DomainError>> GetByIdAsync(long id) {
        logger.LogInformation("Getting Categoria {CategoriaId}", id);

        var categoria = await categoriaRepository.GetByIdAsync(id);
        return categoria is null
            ? Result.Failure<CategoriaDto, DomainError>(NotFound.FromId(id, "Categoría"))
            : Result.Success<CategoriaDto, DomainError>(ToDto(categoria));
    }

    private static CategoriaDto ToDto(Categoria categoria) => new(categoria.Id, categoria.Name);
}
