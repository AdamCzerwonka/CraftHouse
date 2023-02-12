using Azure.Core;
using CraftHouse.Web.Data;
using CraftHouse.Web.DTOs;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class CategoryPage : PageModel
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IValidator<Category> _validator;

    public CategoryPage(ICategoryRepository categoryRepository, IValidator<Category> validator)
    {
        _categoryRepository = categoryRepository;
        _validator = validator;
    }

    public Category? Category { get; set; }

    [BindProperty]
    public CategoryDeleteModel CategoryDelete { get; set; } = null!;

    [BindProperty]
    public CategoryUpdateModel CategoryUpdate { get; set; } = null!;
    
    public string? Error { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        Category = await _categoryRepository.GetCategoryWithProducts(id, cancellationToken);

        if (Category is null)
        {
            throw new InvalidOperationException("Category does not exists");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(CategoryDelete.Id, cancellationToken);
        if (category is null)
        {
            throw new NullReferenceException("Category does not exists");
        }

        var isCategoryEmpty = await _categoryRepository.IsCategoryEmptyAsync(CategoryDelete.Id, cancellationToken);

        if (isCategoryEmpty)
        {
            throw new InvalidOperationException("Cannot delete non empty category");
        }

        await _categoryRepository.DeleteCategoryAsync(category, cancellationToken);

        return RedirectToPage("/Admin/Categories");
    }

    public async Task<IActionResult> OnPostUpdateAsync(CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(CategoryUpdate.Id, cancellationToken);
        if (category is null)
        {
            throw new NullReferenceException("Category does not exists");
        }

        var isCategoryNameTaken = await _categoryRepository
            .CategoryExistsAsync(CategoryUpdate.Name, cancellationToken);
        
        if (isCategoryNameTaken)
        {
            throw new InvalidOperationException("Category name is taken");
        }

        var categoryDto = new CategoryDto
        {
            Name = CategoryUpdate.Name
        };
        var toCheck = categoryDto.MapToCategory();
        var validationResult = await _validator.ValidateAsync(toCheck, cancellationToken);

        if (!validationResult.IsValid)
        {
            Error = validationResult.Errors.First().ToString();
            Category = await _categoryRepository.GetCategoryWithProducts(category.Id, cancellationToken);
            return Page();
        }
        category.Name = CategoryUpdate.Name;
        
        await _categoryRepository.UpdateCategoryAsync(category, cancellationToken);

        return Redirect("/admin/category/" + category.Id);
    }
}

public record CategoryDeleteModel(int Id);

public record CategoryUpdateModel(int Id, string Name);