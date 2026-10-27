using FluentValidation;
using Funko_Api.Infrastructure.Interface;
using Funko_Api.Models;

namespace Funko_Api.Validator;

public class FunkoRequestValidator : AbstractValidator<Funko>, IScopedService {
    public FunkoRequestValidator() {
        RuleFor(funko => funko.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(funko => funko.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(funko => funko.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock must be greater than or equal to 0.");

        RuleFor(funko => funko.Categoria)
            .NotNull().WithMessage("Category is required.");

        When(funko => funko.Categoria is not null, () => {
            RuleFor(funko => funko.Categoria.Name)
                .Cascade(CascadeMode.Stop)
                .Must(name => !string.IsNullOrWhiteSpace(name))
                .WithMessage("Category name is required.")
                .MaximumLength(200).WithMessage("Category name cannot exceed 200 characters.");
        });
    }
}
