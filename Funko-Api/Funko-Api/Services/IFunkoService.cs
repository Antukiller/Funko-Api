using CSharpFunctionalExtensions;
using Funko_Api.Dto;
using Funko_Api.Errors;

namespace Funko_Api.Services;

public interface IFunkoService {
    Task<IReadOnlyList<FunkoDto>> GetAllFunkosAsync();
    Task<IReadOnlyList<FunkoDto>> SearchByNameAsync(string nombre);
    Task<Result<FunkoDto, DomainError>> GetByIdAsync(long id);
    Task<Result<FunkoDto, DomainError>> CreateAsync(CreateFunkoDto request);
    Task<Result<FunkoDto, DomainError>> UpdateAsync(long id, UpdateFunkoDto request);
    Task<Result<bool, DomainError>> DeleteAsync(long id);
}