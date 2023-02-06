﻿using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages.Admin;

public class CategoryPage : PageModel
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryPage(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Category? Category { get; set; }

    [BindProperty]
    public CategoryDeleteModel CategoryDelete { get; set; } = null!;

    [BindProperty]
    public CategoryUpdateModel CategoryUpdate { get; set; } = null!;

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

        category.Name = CategoryUpdate.Name;
        await _categoryRepository.UpdateCategoryAsync(category, cancellationToken);

        return Redirect("/admin/category/" + category.Id);
    }
}

public record CategoryDeleteModel(int Id);

public record CategoryUpdateModel(int Id, string Name);