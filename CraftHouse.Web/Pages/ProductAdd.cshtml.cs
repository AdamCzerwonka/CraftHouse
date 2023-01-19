using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class ProductAdd : PageModel
{
    private readonly AppDbContext _context;

    public ProductAdd(AppDbContext context)
    {
        _context = context;
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


    public void OnGet()
    {
    }

    public async Task OnPostAsync()
    {
        var category = _context.Categories.FirstOrDefault(x => x.Id == CategoryId);
        
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
        
    }
}