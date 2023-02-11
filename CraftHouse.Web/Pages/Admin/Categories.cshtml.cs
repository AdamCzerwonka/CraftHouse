using CraftHouse.Web.Data;
using CraftHouse.Web.DTOs;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class CategoryManagement : PageModel
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<Category> _validator;

    public CategoryManagement(ICategoryRepository categoryRepository, IValidator<Category> validator)
    {
        _categoryRepository = categoryRepository;
        _validator = validator;
    }
    
    [BindProperty]
    public CategoryDto CategoryDto { get; set; } = null!;
    public List<Category> Categories { get; set; } = null!;
    public string? Error { get; set; }

    public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
    {
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);

        return Page();
    }
    
    public async Task<IActionResult> OnPostAddCategoryAsync(CancellationToken cancellationToken)
    {
        var isCategoryExisting = await _categoryRepository.CategoryExistsAsync(CategoryDto.Name, cancellationToken);

        if (isCategoryExisting)
        {
            Error = "That named category already exits";
            Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
            CategoryDto.Name = "";
            return Page();
        }
        
        var category = CategoryDto.MapToCategory();
        var validationResult = await _validator.ValidateAsync(category, cancellationToken);

        if (!validationResult.IsValid)
        {
            Error = validationResult.Errors.First().ToString();
            Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
            CategoryDto.Name = "";
            return Page();
        }
        
        await _categoryRepository.AddCategoryAsync(category, cancellationToken);

        return Redirect("Categories");
    }
}