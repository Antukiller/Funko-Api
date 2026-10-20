using CSharpFunctionalExtensions;
using FluentValidation;
using Funko_Api.Dto;
using Funko_Api.Errors;
using Funko_Api.Infrastructure.Interface;
using Funko_Api.Models;
using Funko_Api.Mapper;
using Funko_Api.Repositories.Categoria;
using Funko_Api.Repositories;

namespace Funko_Api.Services;

public class FunkoService(
    IFunkoRepository funkoRepository,
    ICategoriaRepository categoriaRepository,
    IValidator<Funko> validator,
    ILogger<FunkoService> logger
) : IFunkoService, IScopedService {
    public async Task<IReadOnlyList<FunkoDto>> GetAllFunkosAsync() {
        logger.LogInformation("Getting all Funkos");
        var funkos = await funkoRepository.GetAllAsync();
        return funkos.Select(funko => funko.ToDto()).ToList();
    }

    public async Task<Result<FunkoDto, DomainError>> GetByIdAsync(long id) {
        var result = await funkoRepository.GetByIdAsync(id);
        return result.IsSuccess
            ? Result.Success<FunkoDto, DomainError>(result.Value.ToDto())
            : Result.Failure<FunkoDto, DomainError>(result.Error);
    }

    public async Task<Result<FunkoDto, DomainError>> CreateAsync(CreateFunkoDto request) {
        if (request is null)
            return Result.Failure<FunkoDto, DomainError>(
                ValidationError.Create("La petición de creación no puede ser nula."));

        var categoria = await categoriaRepository.GetByNameAsync(request.CategoriaName);
        if (categoria.IsFailure)
            return Result.Failure<FunkoDto, DomainError>(categoria.Error);

        var funko = request.ToModel(categoria.Value);
        var validation = await validator.ValidateAsync(funko);
        if (!validation.IsValid)
            return Result.Failure<FunkoDto, DomainError>(ToValidationError(validation));

        var result = await funkoRepository.CreateAsync(funko);
        if (result.IsFailure) {
            logger.LogWarning("Could not create Funko {Name}: {Error}", request.Name, result.Error);
            return Result.Failure<FunkoDto, DomainError>(result.Error);
        }

        logger.LogInformation("Created Funko {FunkoId}", result.Value.Id);
        return Result.Success<FunkoDto, DomainError>(result.Value.ToDto());
    }

    public async Task<Result<FunkoDto, DomainError>> UpdateAsync(long id, UpdateFunkoDto request) {
        if (request is null)
            return Result.Failure<FunkoDto, DomainError>(
                ValidationError.Create("La petición de actualización no puede ser nula."));

        var existente = await funkoRepository.GetByIdAsync(id);
        if (existente.IsFailure)
            return Result.Failure<FunkoDto, DomainError>(existente.Error);

        var categoria = await categoriaRepository.GetByNameAsync(request.CategoriaName);
        if (categoria.IsFailure)
            return Result.Failure<FunkoDto, DomainError>(categoria.Error);

        var funko = request.ToModel(categoria.Value);
        var validation = await validator.ValidateAsync(funko);
        if (!validation.IsValid)
            return Result.Failure<FunkoDto, DomainError>(ToValidationError(validation));

        var result = await funkoRepository.UpdateAsync(id, funko);
        if (result.IsFailure) {
            logger.LogWarning("Could not update Funko {FunkoId}: {Error}", id, result.Error);
            return Result.Failure<FunkoDto, DomainError>(result.Error);
        }

        logger.LogInformation("Updated Funko {FunkoId}", id);
        return Result.Success<FunkoDto, DomainError>(result.Value.ToDto());
    }

    public async Task<Result<bool, DomainError>> DeleteAsync(long id) {
        var result = await funkoRepository.DeleteAsync(id);
        if (!result) {
            logger.LogWarning("Could not delete Funko {FunkoId}: not found", id);
            return Result.Failure<bool, DomainError>(NotFound.FromId(id, "Funko"));
        }

        logger.LogInformation("Deleted Funko {FunkoId}", id);
        return Result.Success<bool, DomainError>(true);
    }

    public async Task<IReadOnlyList<CategoriaDto>> GetAllCategoriesAsync() {
        var categories = await categoriaRepository.GetAllAsync();
        return categories.Select(categoria => new CategoriaDto(categoria.Id, categoria.Name)).ToList();
    }

    public async Task<Result<CategoriaDto, DomainError>> GetCategoryByIdAsync(long id) {
        var result = await categoriaRepository.GetByIdAsync(id);
        return result.IsSuccess
            ? Result.Success<CategoriaDto, DomainError>(
                new CategoriaDto(result.Value.Id, result.Value.Name))
            : Result.Failure<CategoriaDto, DomainError>(result.Error);
    }

    private static ValidationError ToValidationError(FluentValidation.Results.ValidationResult result) =>
        ValidationError.WithFieldErrors(result.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray()));

}