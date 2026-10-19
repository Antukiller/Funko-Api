using FluentValidation;
using Funko_Api.Models;

namespace Funko_Api.Validator;

public class CategoriaRequestValidator : AbstractValidator<Categoria> {
    public CategoriaRequestValidator() {
        RuleFor(categoria => categoria.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");
    }
}