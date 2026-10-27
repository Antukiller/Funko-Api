using Funko_Api.Data;
using Microsoft.EntityFrameworkCore;
using CategoriaModel = Funko_Api.Models.Categoria;

namespace Funko_Api.Repositories.Categoria;

/// <summary>
/// Repositorio de categorías sobre Entity Framework Core.
/// </summary>
/// <param name="context">Contexto de base de datos.</param>
public class CategoriaRepository(AppDbContext context) : ICategoriaRepository {
    /// <inheritdoc/>
    public async Task<IReadOnlyList<CategoriaModel>> GetAllAsync() =>
        await context.Categorias
            .AsNoTracking()
            .ToListAsync();

    /// <inheritdoc/>
    public async Task<CategoriaModel?> GetByIdAsync(long id) =>
        await context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

    /// <summary>
    /// Busca una categoría por nombre. Sin <c>AsNoTracking</c> a propósito: el resultado
    /// se usa para crear o actualizar funkos y la entidad debe venir rastreada por el contexto.
    /// </summary>
    /// <inheritdoc/>
    public async Task<CategoriaModel?> GetByNameAsync(string name) =>
        await context.Categorias
            .FirstOrDefaultAsync(item => item.Name == name);
}
