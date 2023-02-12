using CraftHouse.Web.Entities;
using FluentValidation;

namespace CraftHouse.Web.Validators;

public class OptionValueValidator : AbstractValidator<OptionValue>
{
    public OptionValueValidator()
    {
        RuleFor(optionValue => optionValue.Value).NotEmpty().MinimumLength(3);
        RuleFor(optionValue => optionValue.Price).NotEmpty().Must(ValidationMethods.IsPriceValid);
        RuleFor(optionValue => optionValue.OptionId).NotEmpty().GreaterThan(0);
        
    }
}