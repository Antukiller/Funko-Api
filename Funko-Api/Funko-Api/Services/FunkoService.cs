using CSharpFunctionalExtensions;
using FluentValidation;
using Funko_Api.Dto;
using Funko_Api.Errors;
using Funko_Api.Infrastructure.Interface;
using Funko_Api.Mapper;
using Funko_Api.Models;
using Funko_Api.Repositories;
using Funko_Api.Repositories.Categoria;

namespace Funko_Api.Services;

public class FunkoService(
    IFunkoRepository funkoRepository,
    ICategoriaRepository categoriaRepository,
    IValidator<Funko> funkoValidator,
    IValidator<CreateFunkoDto> createValidator,
    IValidator<UpdateFunkoDto> updateValidator,
    ILogger<FunkoService> logger
) : IFunkoService, IScopedService {
    public async Task<IReadOnlyList<FunkoDto>> GetAllFunkosAsync() {
        logger.LogInformation("Getting all Funkos");
        var funkos = await funkoRepository.GetAllAsync();
        return funkos.Select(funko => funko.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<FunkoDto>> SearchByNameAsync(string nombre) {
        logger.LogInformation("Searching Funkos by name {Nombre}", nombre);
        var funkos = await funkoRepository.SearchByNameAsync(nombre);
        return funkos.Select(funko => funko.ToDto()).ToList();
    }

    public async Task<Result<FunkoDto, DomainError>> GetByIdAsync(long id) {
        var funko = await funkoRepository.GetByIdAsync(id);
        return funko is null
            ? Result.Failure<FunkoDto, DomainError>(NotFound.FromId(id, "Funko"))
            : Result.Success<FunkoDto, DomainError>(funko.ToDto());
    }

    public async Task<Result<FunkoDto, DomainError>> CreateAsync(CreateFunkoDto request) {
        if (request is null)
            return Result.Failure<FunkoDto, DomainError>(
                ValidationError.Create("La petición de creación no puede ser nula."));

        var requestValidation = await createValidator.ValidateAsync(request);
        if (!requestValidation.IsValid)
            return Result.Failure<FunkoDto, DomainError>(ToValidationError(requestValidation));

        if (await funkoRepository.ExistsByNameAsync(request.Name))
            return Result.Failure<FunkoDto, DomainError>(
                ConflictError.Duplicate("Funko", request.Name));

        var categoria = await categoriaRepository.GetByNameAsync(request.CategoriaName);
        if (categoria is null)
            return Result.Failure<FunkoDto, DomainError>(CategoriaNotFound(request.CategoriaName));

        var funko = request.ToModel(categoria);
        var validation = await funkoValidator.ValidateAsync(funko);
        if (!validation.IsValid)
            return Result.Failure<FunkoDto, DomainError>(ToValidationError(validation));

        var creado = await funkoRepository.CreateAsync(funko);
        logger.LogInformation("Created Funko {FunkoId}", creado.Id);
        return Result.Success<FunkoDto, DomainError>(creado.ToDto());
    }

    public async Task<Result<FunkoDto, DomainError>> UpdateAsync(long id, UpdateFunkoDto request) {
        if (request is null)
            return Result.Failure<FunkoDto, DomainError>(
                ValidationError.Create("La petición de actualización no puede ser nula."));

        var requestValidation = await updateValidator.ValidateAsync(request);
        if (!requestValidation.IsValid)
            return Result.Failure<FunkoDto, DomainError>(ToValidationError(requestValidation));

        var existente = await funkoRepository.GetByIdAsync(id);
        if (existente is null)
            return Result.Failure<FunkoDto, DomainError>(NotFound.FromId(id, "Funko"));

        if (await funkoRepository.ExistsByNameAsync(request.Name, id))
            return Result.Failure<FunkoDto, DomainError>(
                ConflictError.Duplicate("Funko", request.Name));

        var categoria = await categoriaRepository.GetByNameAsync(request.CategoriaName);
        if (categoria is null)
            return Result.Failure<FunkoDto, DomainError>(CategoriaNotFound(request.CategoriaName));

        var funko = request.ToModel(categoria);
        var validation = await funkoValidator.ValidateAsync(funko);
        if (!validation.IsValid)
            return Result.Failure<FunkoDto, DomainError>(ToValidationError(validation));

        var actualizado = await funkoRepository.UpdateAsync(id, funko);
        if (actualizado is null) {
            logger.LogWarning("Could not update Funko {FunkoId}: not found", id);
            return Result.Failure<FunkoDto, DomainError>(NotFound.FromId(id, "Funko"));
        }

        logger.LogInformation("Updated Funko {FunkoId}", id);
        return Result.Success<FunkoDto, DomainError>(actualizado.ToDto());
    }

    public async Task<Result<FunkoDto, DomainError>> AdjustStockAsync(long id, int cantidad) {
        if (cantidad == 0)
            return Result.Failure<FunkoDto, DomainError>(
                ValidationError.Create("La cantidad debe ser distinta de cero."));

        var funko = await funkoRepository.GetByIdAsync(id);
        if (funko is null)
            return Result.Failure<FunkoDto, DomainError>(NotFound.FromId(id, "Funko"));

        var nuevoStock = funko.Stock + cantidad;
        if (nuevoStock < 0)
            return Result.Failure<FunkoDto, DomainError>(
                BusinessRuleError.StockInsuficiente(funko.Name, funko.Stock, Math.Abs(cantidad)));

        var actualizado = await funkoRepository.UpdateStockAsync(id, nuevoStock);
        if (actualizado is null) {
            logger.LogWarning("Could not adjust stock of Funko {FunkoId}: not found", id);
            return Result.Failure<FunkoDto, DomainError>(NotFound.FromId(id, "Funko"));
        }

        logger.LogInformation("Adjusted stock of Funko {FunkoId} to {Stock}", id, nuevoStock);
        return Result.Success<FunkoDto, DomainError>(actualizado.ToDto());
    }

    public async Task<Result<bool, DomainError>> DeleteAsync(long id) {
        var deleted = await funkoRepository.DeleteAsync(id);
        if (!deleted) {
            logger.LogWarning("Could not delete Funko {FunkoId}: not found", id);
            return Result.Failure<bool, DomainError>(NotFound.FromId(id, "Funko"));
        }

        logger.LogInformation("Deleted Funko {FunkoId}", id);
        return Result.Success<bool, DomainError>(true);
    }

    private static NotFound CategoriaNotFound(string nombre) =>
        new($"La categoría '{nombre}' no existe");

    private static ValidationError ToValidationError(FluentValidation.Results.ValidationResult result) =>
        ValidationError.WithFieldErrors(result.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray()));
}
