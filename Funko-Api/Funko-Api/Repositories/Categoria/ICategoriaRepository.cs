using CSharpFunctionalExtensions;
using Funko_Api.Errors;

namespace Funko_Api.Repositories.Categoria;

public interface ICategoriaRepository {
    Task<IReadOnlyList<Models.Categoria>> GetAllAsync();

    Task<Result<Models.Categoria, DomainError>> GetByIdAsync(long id);
}