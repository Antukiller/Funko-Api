using FluentValidation;

namespace Funko_Api.Validator;

public class CategoriaRequestValidator : AbstractValidator<CategoriaRequestDto> {
    public CategoriaRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");
    }
    
}