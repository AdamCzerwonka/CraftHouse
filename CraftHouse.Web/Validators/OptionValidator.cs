using CraftHouse.Web.Entities;
using FluentValidation;

namespace CraftHouse.Web.Validators;

public class OptionValidator : AbstractValidator<Option>
{
    public OptionValidator()
    {
        RuleFor(option => option.Name).NotEmpty().MinimumLength(3);
        RuleFor(option => option.MaxOccurs).NotEmpty().GreaterThanOrEqualTo(0);
        RuleFor(option => option.ProductId).NotEmpty().GreaterThan(0);
    }
}