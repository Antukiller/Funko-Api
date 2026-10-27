using FluentValidation;
using Funko_Api.Models;

namespace Funko_Api.Validators;

public class FunkoRequestValidator : AbstractValidator<Funko> {
    public FunkoRequestValidator() {
        RuleFor(funko => funko.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200).WithMessage("El nombre no puede superar los 200 caracteres.");

        RuleFor(funko => funko.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor que 0.");

        RuleFor(funko => funko.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.");

        RuleFor(funko => funko.Categoria)
            .NotNull().WithMessage("La categoría es obligatoria.");

        When(funko => funko.Categoria is not null, () => {
            RuleFor(funko => funko.Categoria.Name)
                .Cascade(CascadeMode.Stop)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("El nombre de la categoría es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre de la categoría no puede superar los 200 caracteres.");
        });
    }
}
