using FluentValidation;
using Funko_Api.Dtos;

namespace Funko_Api.Validators;

/// <summary>
/// Validador del DTO de creación de funkos.
/// </summary>
public class CreateFunkoValidator : AbstractValidator<CreateFunkoDto> {
    /// <summary>
    /// Define las reglas de validación de una creación.
    /// </summary>
    public CreateFunkoValidator() {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("El nombre es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres.");

        RuleFor(request => request.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor que 0.");

        RuleFor(request => request.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

        RuleFor(request => request.CategoriaName)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("El nombre de la categoría es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre de la categoría no puede superar los 200 caracteres.");
    }
}
