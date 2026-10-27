using FluentValidation;
using Funko_Api.Dto;
using Funko_Api.Infrastructure.Interface;

namespace Funko_Api.Validator;

public class CreateFunkoValidator : AbstractValidator<CreateFunkoDto>, IScopedService {
    public CreateFunkoValidator() {
        RuleFor(request => request.Name)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(request => request.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(request => request.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock must be greater than or equal to 0.");

        RuleFor(request => request.CategoriaName)
            .Cascade(CascadeMode.Stop)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage("Category name is required.")
            .MaximumLength(200).WithMessage("Category name cannot exceed 200 characters.");
    }
}
