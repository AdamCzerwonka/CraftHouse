using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class CategoryManagement : PageModel
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryManagement(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    
    public List<Category> Categories { get; set; } = null!;

    public string? Error { get; set; }

    [BindProperty]
    public string CategoryName { get; set; } = null!;

    public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
    {
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);

        return Page();
    }
    
    public async Task<IActionResult> OnPostAddCategoryAsync(CancellationToken cancellationToken)
    {
        var isCategoryExisting = await _categoryRepository.CategoryExistsAsync(CategoryName, cancellationToken);
        
        if (CategoryName.Length < 3 || isCategoryExisting)
        {
            Error = "That named category already exits";
            if (CategoryName.Length < 3)
            {
                Error = "Incorrect category name";
            }

            Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
            CategoryName = "";
            return Page();
        }

        var category = new Category
        {
            Name = CategoryName
        };
        
        await _categoryRepository.UpdateCategoryAsync(category, cancellationToken);

        return Redirect("Categories");
    }
}