using FluentValidation;

namespace Funko_Api.Validator;

public class FunkoRequestValidator : AbstractValidator<FunkoRequestDto> {
{
    public FunkoRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock must be greater than or equal to 0.");
            
    }
    
}