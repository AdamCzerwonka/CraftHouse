using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class ProductManagement : PageModel
{
    private readonly AppDbContext _context;
    private readonly ILogger _logger;

    public ProductManagement(AppDbContext context, ILogger<ProductManagement> logger)
    {
        _context = context;
        _logger = logger;
    }

    [BindProperty]
    public string Name { get; set; } = null!;

    [BindProperty]
    public bool IsAvailable { get; set; }

    [BindProperty]
    public float Price { get; set; }

    [BindProperty]
    public string Description { get; set; } = null!;

    [BindProperty]
    public int CategoryId { get; set; }

    public List<Category> Categories { get; set; } = null!;

    [BindProperty]
    public int ProductId { get; set; }

    public List<Product> Products { get; set; } = null!;

    public IActionResult OnGet()
    {
        Categories = _context.Categories.Where(x => x.DeletedAt == null).ToList();
        Products = _context.Products.Where(x => x.DeletedAt == null).ToList();

        return Page();
    }

    public async Task<RedirectResult> OnPostProductAsync()
    {
        var category = _context.Categories
            .Where(x => x.DeletedAt == null)
            .FirstOrDefault(x => x.Id == CategoryId);

        var product = new Product()
        {
            Name = Name,
            IsAvailable = IsAvailable,
            Price = Price,
            Description = Description,
            Category = category!
        };

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        return Redirect("/admin/Products");
    }
    
    public async Task<RedirectResult> OnPostRemoveAsync()
    {
        var product = _context.Products.Where(x => x.DeletedAt == null).FirstOrDefault(x => x.Id == ProductId);
        
        product!.DeletedAt = DateTime.Now;
        
        var options = _context.Products.Include(x => x.Options).FirstOrDefault(x => x.Id == ProductId);
        
        foreach (var option in options.Options)
        {
            option.DeletedAt = DateTime.Now;
        }

        _context.Products.Update(product);
        await _context.SaveChangesAsync();

        return Redirect("/admin/Products");
    }
    
    
}
