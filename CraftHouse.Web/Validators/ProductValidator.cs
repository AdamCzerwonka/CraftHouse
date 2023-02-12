using CraftHouse.Web.Entities;
using FluentValidation;

namespace CraftHouse.Web.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(product => product.Name).NotEmpty().MinimumLength(3);
        RuleFor(product => product.IsAvailable).NotNull();
        RuleFor(product => product.Price).NotEmpty().GreaterThanOrEqualTo(0).Must(ValidationMethods.IsPriceValid);
        RuleFor(product => product.Description).NotEmpty().MinimumLength(3);
        RuleFor(product => product.CategoryId).NotEmpty().GreaterThan(0);
    }
}