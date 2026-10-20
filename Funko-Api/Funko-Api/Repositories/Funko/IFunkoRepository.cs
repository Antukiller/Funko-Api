using CSharpFunctionalExtensions;
using Funko_Api.Errors;
using Funko_Api.Models;

namespace Funko_Api.Repositories;

public interface IFunkoRepository {
     Task<IReadOnlyList<Funko>> GetAllAsync();
     Task<Result<Funko, DomainError>> GetByIdAsync(long id);
     Task<Result<Funko, DomainError>> CreateAsync(Funko funko);
     Task<Result<Funko, DomainError>> UpdateAsync(long id, Funko funko);
     Task<bool> DeleteAsync(long id);
     
}