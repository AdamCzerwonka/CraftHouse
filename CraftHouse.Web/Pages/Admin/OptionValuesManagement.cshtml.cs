using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class OptionValuesManagement : PageModel
{
    private readonly IOptionValueRepository _optionValueRepository;
    private readonly IOptionRepository _optionRepository;

    public OptionValuesManagement(IOptionValueRepository optionValueRepository, IOptionRepository optionRepository)
    {
        _optionValueRepository = optionValueRepository;
        _optionRepository = optionRepository;
    }
    
    [BindProperty]
    public int OptionId { get; set; }
    
    [BindProperty]
    public string OptionValue { get; set; } = null!;
    
    [BindProperty]
    public string OldOptionValue { get; set; } = null!;
    
    [BindProperty]
    public float OptionPrice { get; set; }
    
    [BindProperty]
    public string OptionName { get; set; } = null!;

    public List<OptionValue> OptionValues { get; set; } = null!;

    public Option Option { get; set; }
    
    public async Task OnGet(int optionId, CancellationToken cancellationToken)
    {
        OptionId = optionId;
        Option = await _optionRepository.GetOptionByIdAsync(OptionId, cancellationToken);
        
        if (Option is null)
        {
            throw new NullReferenceException("Option does not exists");
        }
        
        OptionValues = await _optionValueRepository.GetOptionValuesByOptionIdAsync(OptionId, cancellationToken);
        OptionName = Option!.Name;
    }

    public async Task<IActionResult> OnPostRemoveAsync(CancellationToken cancellationToken)
    {
        var optionValueToDelete = await _optionValueRepository.GetOptionValueAsync(OptionId, OptionValue, cancellationToken);
        
        if (optionValueToDelete is null)
        {
            throw new NullReferenceException("Option value does not exists");
        }
        await _optionValueRepository.RemoveOptionValueAsync(OptionId, OptionValue, cancellationToken);
        
        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }
    
    public async Task<IActionResult> OnPostOptionNameAsync(CancellationToken cancellationToken)
    {
        var option = await _optionRepository.GetOptionByIdAsync(OptionId, cancellationToken);

        if (option is null)
        {
            throw new NullReferenceException("Option does not exists");
        }
        
        option!.Name = OptionName;

        await _optionRepository.UpdateOptionAsync(option, cancellationToken);
        
        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }
    
    public async Task<IActionResult> OnPostNewValueAsync(CancellationToken cancellationToken)
    {
        var option = await _optionRepository.GetOptionByIdAsync(OptionId, cancellationToken);
        
        if (option is null)
        {
            throw new NullReferenceException("Option does not exists");
        }

        var newValue = new OptionValue()
        {
            Value = OptionValue,
            Price = OptionPrice,
            OptionId = option.Id
        };

        await _optionValueRepository.AddNewOptionValueAsync(newValue, cancellationToken);

        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }
    
    public async Task<IActionResult> OnPostUpdateValueAsync(CancellationToken cancellationToken)
    {
        var value = await _optionValueRepository.GetOptionValueAsync(OptionId, OldOptionValue, cancellationToken);
        
        if (value is null)
        {
            throw new NullReferenceException("Option value does not exists");
        }

        await _optionValueRepository.UpdateOptionValueFieldsAsync(OptionId, OldOptionValue, OptionValue, OptionPrice,
            cancellationToken);

        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }
    
}