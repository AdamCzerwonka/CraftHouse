using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class CategoryManagement : PageModel
{
    private readonly AppDbContext _context;

    public CategoryManagement(AppDbContext context)
    {
        _context = context;
    }
    
    public List<Category> Categories { get; set; } = null!;

    public string? Error { get; set; }

    [BindProperty]
    public string CategoryName { get; set; } = null!;

    private bool CategoryExists(string name)
        => _context.Categories.Any(x => x.Name == name);

    public void OnGet()
    {
        Categories = _context.Categories.Where(x => x.DeletedAt == null).ToList();
    }
    
    public async Task<IActionResult> OnPostAddCategoryAsync()
    {
        if (CategoryName.Length < 3 || CategoryExists(CategoryName))
        {
            Error = "That named category already exits";
            if (CategoryName.Length < 3)
            {
                Error = "Incorrect category name";
            }

            Categories = _context.Categories.Where(x => x.DeletedAt == null).ToList();
            CategoryName = "";
            return Page();
        }

        var category = new Category
        {
            Name = CategoryName
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        return Redirect("Categories");
    }
}