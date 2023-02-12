using CraftHouse.Web.Entities;
using FluentValidation;

namespace CraftHouse.Web.Validators;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(product => product.Name).NotEmpty().MinimumLength(3);
        RuleFor(product => product.IsAvailable).NotNull();
        RuleFor(product => product.Price).NotEmpty().Must(BeAValidPrice);
        RuleFor(product => product.Description).NotEmpty().MinimumLength(3);
    }

    private static bool BeAValidPrice(float price)
    {
        var toCheck = price.ToString();
        if (toCheck.IndexOf(",") == -1) return true;
        var precision = toCheck.Length - toCheck.IndexOf(",") - 1;
        return precision is 2 or 1;
    }
}