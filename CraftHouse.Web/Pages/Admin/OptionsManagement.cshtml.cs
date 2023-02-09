using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class OptionsManagement : PageModel
{
    private readonly IOptionRepository _optionRepository;
    private readonly IProductRepository _productRepository;

    public OptionsManagement (IOptionRepository optionRepository, IProductRepository productRepository)
    {
        _optionRepository = optionRepository;
        _productRepository = productRepository;
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

    public async Task OnGet(int productId, int optionNumber, CancellationToken cancellationToken)
    {
        OptionNumber = optionNumber;
        ProductId = productId;
        ExistingOptions = await _optionRepository.GetOptionsByProductIdAsync(ProductId, cancellationToken);
    }
    
    public async Task<IActionResult> OnPostOptionAsync(CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(ProductId, cancellationToken);
        
        if (product is null)
        {
            throw new NullReferenceException("Product does not exists");
        }
        
        ICollection<OptionValue> optionValues = OptionValues.Select((t, i) => new OptionValue() { Value = t, Price = OptionPrices[i] }).ToList();

        var option = new Option()
        {
            Name = Name,
            MaxOccurs = MaxOccurs,
            OptionValues = optionValues,
            ProductId = product.Id 
        };

        await _optionRepository.AddOptionAsync(option, cancellationToken);

        return Redirect($"/admin/OptionsManagement/{ProductId}?optionNumber={1}");
    }

    public async Task<IActionResult> OnPostRemoveAsync(CancellationToken cancellationToken)
    {
        ExistingOptions = await _optionRepository.GetOptionsByProductIdAsync(ProductId, cancellationToken);

        foreach (var option in ExistingOptions)
        {
            var isDeleted = await _optionRepository.IsOptionDeletedAsync(option, cancellationToken);
            if (option.Id != OptionId || isDeleted) continue;
            await _optionRepository.DeleteOptionAsync(option, cancellationToken);
            break;
        }
        
        return Redirect($"/admin/OptionsManagement/{ProductId}?optionNumber={1}");
    }
}