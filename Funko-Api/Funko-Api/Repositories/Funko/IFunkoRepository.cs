using Funko_Api.Models;

namespace Funko_Api.Repositories;

public interface IFunkoRepository {
    Task<IReadOnlyList<Funko>> GetAllAsync();

    Task<IReadOnlyList<Funko>> SearchByNameAsync(string nombre);

    Task<Funko?> GetByIdAsync(long id);

    Task<Funko> CreateAsync(Funko funko);

    Task<Funko?> UpdateAsync(long id, Funko funko);

    Task<bool> DeleteAsync(long id);
}
