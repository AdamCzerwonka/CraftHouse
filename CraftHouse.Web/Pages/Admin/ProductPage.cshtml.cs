using System.Collections;
using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class ProductPage : PageModel
{
    private readonly AppDbContext _context;

    public ProductPage(AppDbContext context)
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

    [BindProperty]
    public int ProductId { get; set; }

    public List<Category> Categories { get; set; } = null!;
    
    public int OptionNumber { get; set; }

    public void OnGet(int productId, int optionNumber)
    {
        OptionNumber = optionNumber;
        ProductId = productId;
        Categories = _context.Categories.Where(x => x.DeletedAt == null).ToList();
    }

    public async Task<IActionResult> OnPostEditAsync(int optionNumber)
    {
        OptionNumber = optionNumber;

        var product = _context.Products
            .Where(x => x.DeletedAt == null)
            .FirstOrDefault(x => x.Id == ProductId)!;
        var category = _context.Categories
            .Where(x => x.DeletedAt == null)
            .FirstOrDefault(x => x.Id == CategoryId);

        product!.Name = Name;
        product.IsAvailable = IsAvailable;
        product.Price = Price;
        product.Description = Description;
        product.Category = category!;

        _context.Update(product);
        await _context.SaveChangesAsync();

        return Redirect($"/admin/ProductPage/{ProductId}?optionNumber={OptionNumber}");
    }
}