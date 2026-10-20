using CSharpFunctionalExtensions;
using Funko_Api.Data;
using Funko_Api.Errors;
using Funko_Api.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using CategoriaModel = Funko_Api.Models.Categoria;

namespace Funko_Api.Repositories.Categoria;

public class CategoriaRepository(AppDbContext context) : ICategoriaRepository, IScopedService {
    public async Task<IReadOnlyList<CategoriaModel>> GetAllAsync() =>
        await context.Categorias
            .AsNoTracking()
            .ToListAsync();

    public async Task<Result<CategoriaModel, DomainError>> GetByIdAsync(long id) {
        var categoria = await context.Categorias
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        return categoria is not null
            ? Result.Success<CategoriaModel, DomainError>(categoria)
            : Result.Failure<CategoriaModel, DomainError>(NotFound.FromId(id, "Categoría"));
    }
}