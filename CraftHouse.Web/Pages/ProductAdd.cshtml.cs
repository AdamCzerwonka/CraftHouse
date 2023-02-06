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

    public List<Category> Categories { get; set; } = null!;

    [BindProperty]
    public int ProductId { get; set; }

    public List<Product> Products { get; set; } = null!;

    [BindProperty]
    public string FirstOptionValue { get; set; } = null!;

    [BindProperty]
    public int FirstOptionPrice { get; set; }

    [BindProperty]
    public string SecondOptionValue { get; set; } = null!;

    [BindProperty]
    public int SecondOptionPrice { get; set; }

    [BindProperty]
    public string ThirdOptionValue { get; set; } = null!;

    [BindProperty]
    public int ThirdOptionPrice { get; set; }

    public IActionResult OnGet()
    {
        Categories = _context.Categories.ToList();

        Products = _context.Products.ToList();
        
        return Page();
    }
    
    public async Task<RedirectResult> OnPostProductAsync()
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

        return Redirect("/ProductAdd");
    }

    public async Task<RedirectResult> OnPostCategoryAsync()
    {
        var category = new Category()
        {
            Name = Name
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        
        return Redirect("/ProductAdd");
    }

    public async Task<RedirectResult> OnPostOptionAsync()
    {
        var product = _context.Products.FirstOrDefault(x => x.Id == ProductId);

        ICollection<OptionValue> optionValuesCollection = new List<OptionValue>();
        var firstOption = new OptionValue()
        {
            Value = FirstOptionValue,
            Price = FirstOptionPrice
        };
        optionValuesCollection.Add(firstOption);
        var secondOption = new OptionValue()
        {
            Value = SecondOptionValue,
            Price = SecondOptionPrice
        };
        optionValuesCollection.Add(secondOption);
        var thirdOption = new OptionValue()
        {
            Value = ThirdOptionValue,
            Price = ThirdOptionPrice
        };
        optionValuesCollection.Add(thirdOption);

        var option = new Option()
        {
            Name = Name,
            Products = new[] { product! },
            OptionValues = optionValuesCollection
        };

        await _context.Options.AddAsync(option);
        await _context.SaveChangesAsync();
        
        return Redirect("/ProductAdd");
    }
}