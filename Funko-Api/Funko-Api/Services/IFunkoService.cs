using CSharpFunctionalExtensions;
using Funko_Api.Dto;
using Funko_Api.Errors;
using Funko_Api.Models;

namespace Funko_Api.Services;

public interface IFunkoService {
    Task<IReadOnlyList<FunkoDto>> GetAllFunkosAsync();
    Task<Result<FunkoDto, DomainError>> GetByIdAsync(long id);
    Task<Result<FunkoDto, DomainError>> CreateAsync(CreateFunkoDto request);
    Task<Result<FunkoDto, DomainError>> UpdateAsync(long id, UpdateFunkoDto request);
    Task<Result<bool, DomainError>> DeleteAsync(long id);
    Task<IReadOnlyList<CategoriaDto>> GetAllCategoriesAsync();
    Task<Result<CategoriaDto, DomainError>> GetCategoryByIdAsync(long id);
}