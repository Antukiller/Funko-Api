using Funko_Api.Models;

namespace Funko_Api.Repositories;

public interface IFunkoRepository {
    Task<IReadOnlyList<Funko>> GetAllAsync();

    Task<IReadOnlyList<Funko>> SearchByNameAsync(string nombre);

    Task<bool> ExistsByNameAsync(string name, long? excludeId = null);

    Task<Funko?> GetByIdAsync(long id);

    Task<Funko> CreateAsync(Funko funko);

    Task<Funko?> UpdateAsync(long id, Funko funko);

    Task<Funko?> UpdateStockAsync(long id, int stock);

    Task<bool> DeleteAsync(long id);
}
