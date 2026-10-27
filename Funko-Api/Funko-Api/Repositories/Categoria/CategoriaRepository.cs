using Funko_Api.Data;
using Microsoft.EntityFrameworkCore;
using CategoriaModel = Funko_Api.Models.Categoria;

namespace Funko_Api.Repositories.Categoria;

public class CategoriaRepository(AppDbContext context) : ICategoriaRepository {
    public async Task<IReadOnlyList<CategoriaModel>> GetAllAsync() =>
        await context.Categorias
            .AsNoTracking()
            .ToListAsync();

    public async Task<CategoriaModel?> GetByIdAsync(long id) =>
        await context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

    /// <summary>
    /// Busca una categoría por nombre. Sin <c>AsNoTracking</c> a propósito: el resultado
    /// se usa para crear o actualizar funkos y la entidad debe venir rastreada por el contexto.
    /// </summary>
    public async Task<CategoriaModel?> GetByNameAsync(string name) =>
        await context.Categorias
            .FirstOrDefaultAsync(item => item.Name == name);
}
