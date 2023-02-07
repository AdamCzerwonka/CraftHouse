using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class OptionValuesManagement : PageModel
{
    private readonly AppDbContext _context;

    public OptionValuesManagement(AppDbContext context)
    {
        _context = context;
    }
    
    [BindProperty]
    public int OptionId { get; set; }
    
    [BindProperty]
    public int ProductId { get; set; }

    [BindProperty]
    public string OptionValue { get; set; } = null!;
    
    [BindProperty]
    public string OldOptionValue { get; set; } = null!;
    
    [BindProperty]
    public float OptionPrice { get; set; }
    
    [BindProperty]
    public string OptionName { get; set; } = null!;

    public List<OptionValue> OptionValues { get; set; } = null!;



    public void OnGet(int optionId)
    {
        OptionId = optionId;
        OptionValues = _context.OptionValues.Where(x => x.OptionId == OptionId).ToList();
        OptionName = _context.Options.FirstOrDefault(x => x.Id == OptionId)!.Name;
    }

    public async Task<IActionResult> OnPostRemoveAsync()
    {
        OptionValues = _context.OptionValues.Where(x => x.OptionId == OptionId).ToList();

        foreach (var option in OptionValues)      
        {
            if (option.OptionId == OptionId && option.Value == OptionValue)
            {
                _context.OptionValues.Remove(option);
            }
        }

        await _context.SaveChangesAsync();

        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }
    
    public async Task<IActionResult> OnPostOptionNameAsync()
    {
        var option = _context.Options.FirstOrDefault(x => x.Id == OptionId);

        option!.Name = OptionName;

        _context.Options.Update(option);
        await _context.SaveChangesAsync();

        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }
    
    public async Task<IActionResult> OnPostNewValueAsync()
    {
        var option = _context.Options.FirstOrDefault(x => x.Id == OptionId);

        var newValue = new OptionValue()
        {
            Value = OptionValue,
            Price = OptionPrice,
            Option = option!
        };

        await _context.OptionValues.AddAsync(newValue);
        await _context.SaveChangesAsync();

        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }
    
    public async Task<IActionResult> OnPostUpdateValueAsync()
    {
        var value = _context.OptionValues.FirstOrDefault(x => x.OptionId == OptionId && x.Value == OldOptionValue);

        _context.OptionValues.Remove(value!);

        var option = _context.Options.FirstOrDefault(x => x.Id == OptionId);

        var newValue = new OptionValue()
        {
            Value = OptionValue,
            Price = OptionPrice,
            Option = option!
        };
        
        await _context.OptionValues.AddAsync(newValue);
        await _context.SaveChangesAsync();

        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }


}