using CraftHouse.Web.Entities;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace CraftHouse.Web.Validators;

public class CategoryValidator : AbstractValidator<Category>
{
    public CategoryValidator()
    {
        RuleFor(category => category.Name).NotEmpty().MinimumLength(3);
    }
}