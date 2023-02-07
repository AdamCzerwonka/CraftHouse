using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class OptionsManagement : PageModel
{
    private readonly AppDbContext _context;

    public OptionsManagement (AppDbContext context)
    {
        _context = context;
    }
    
    [BindProperty]
    public string Name { get; set; } = null!;

    [BindProperty]
    public int ProductId { get; set; }
    
    [BindProperty]
    public int OptionId { get; set; }
    
    [BindProperty]
    public int MaxOccurs { get; set; }

    [BindProperty]
    public List<string> OptionValues { get; set; } = null!;

    [BindProperty]
    public List<float> OptionPrices { get; set; } = null!;
    
    public List<Option> ExistingOptions { get; set; } = null!;

    public int OptionNumber { get; set; }

    public void OnGet(int productId, int optionNumber)
    {
        OptionNumber = optionNumber;
        ProductId = productId;
        ExistingOptions = _context.Options.Where(x => x.Products.Any(x => x.Id == ProductId)).Where(x => x.DeletedAt == null).ToList();
    }
    
    public async Task<IActionResult> OnPostOptionAsync()
    {
        var product = _context.Products.FirstOrDefault(x => x.Id == ProductId);

        ICollection<OptionValue> optionValues = OptionValues.Select((t, i) => new OptionValue() { Value = t, Price = OptionPrices[i] }).ToList();

        var option = new Option()
        {
            Name = Name,
            MaxOccurs = MaxOccurs,
            OptionValues = optionValues,
            Products = new[] { product! }
        };

        await _context.Options.AddAsync(option);
        await _context.SaveChangesAsync();

        return Redirect($"/admin/OptionsManagement/{ProductId}?optionNumber={1}");
    }

    public async Task<IActionResult> OnPostRemoveAsync()
    {
        ExistingOptions = _context.Options.Where(x => x.Products.Any(x => x.Id == ProductId)).Where(x => x.DeletedAt == null).ToList();

        foreach (var option in ExistingOptions)
        {
            if (option.Id != OptionId) continue;
            option.UpdatedAt = DateTime.Now;
            option.DeletedAt = DateTime.Now;
            _context.Options.Update(option);
            break;
        }
        
        await _context.SaveChangesAsync();
        
        return Redirect($"/admin/OptionsManagement/{ProductId}?optionNumber={1}");
    }
}