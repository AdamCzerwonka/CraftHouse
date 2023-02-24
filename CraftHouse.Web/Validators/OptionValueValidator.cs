using CraftHouse.Web.Entities;
using FluentValidation;

namespace CraftHouse.Web.Validators;

public class OptionValueValidator : AbstractValidator<OptionValue>
{
    public OptionValueValidator()
    {
        RuleFor(optionValue => optionValue.Value).NotEmpty().MinimumLength(3);
        RuleFor(optionValue => optionValue.Price).GreaterThanOrEqualTo(0);
        RuleFor(optionValue => optionValue.OptionId).NotEmpty().GreaterThan(0);
        
    }
}