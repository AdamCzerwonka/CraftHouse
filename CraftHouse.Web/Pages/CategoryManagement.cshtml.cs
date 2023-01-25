using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class CategoryManagement : PageModel
{
    private readonly AppDbContext _context;

    public CategoryManagement(AppDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public int CategoryId { get; set; }

    public List<Category> Categories { get; set; } = null!;

    public List<Product> Products { get; set; } = null!;

    public bool Error { get; set; } = false;

    public string ErrorMessage { get; set; } = "You can not delete category when any product is in it";

    [BindProperty]
    public string CategoryName { get; set; } = null!;

    public void OnGet()
    {
        Categories = _context.Categories.Where(x => x.DeletedAt == null).ToList();
        Products = _context.Products.ToList();
    }

    public async Task<IActionResult> OnPostDeleteCategoryAsync()
    {
        Categories = _context.Categories.Where(x => x.DeletedAt == null).ToList();

        var containsProducts =
            _context.Products.Any(product => product.CategoryId == CategoryId && product.DeletedAt == null);

        var category = _context.Categories.FirstOrDefault(x => x.Id == CategoryId);

        if (containsProducts || category == null)
        {
            if (category == null)
            {
                ErrorMessage = "This category does not exists";
            }

            Error = true;
            return Page();
        }

        category.DeletedAt = DateTime.Now;

        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return Redirect("/CategoryManagement");
    }

    public async Task<IActionResult> OnPostAddCategoryAsync()
    {
        Categories = _context.Categories.Where(x => x.DeletedAt == null).ToList();

        if (CategoryName.Length < 3)
        {
            ErrorMessage = "Incorrect category name";
            Error = true;
            return Page();
        }

        var category = new Category()
        {
            Name = CategoryName
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return Redirect("/CategoryManagement");
    }

    public async Task<IActionResult> OnPostRenameCategoryAsync()
    {
        Categories = _context.Categories.Where(x => x.DeletedAt == null).ToList();

        if (CategoryName.Length < 3)
        {
            ErrorMessage = "Incorrect category name";
            Error = true;
            return Page();
        }
        
        var category = _context.Categories.FirstOrDefault(x => x.Id == CategoryId);

        if (category == null)
        {
            ErrorMessage = "This category does not exists";
            Error = true;
            return Page();
        }

        category.Name = CategoryName;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();

        return Redirect("/CategoryManagement");
    }
}