using CategoriaModel = Funko_Api.Models.Categoria;

namespace Funko_Api.Repositories.Categoria;

public interface ICategoriaRepository {
    Task<IReadOnlyList<CategoriaModel>> GetAllAsync();

    Task<CategoriaModel?> GetByIdAsync(long id);

    Task<CategoriaModel?> GetByNameAsync(string name);
}
