using CraftHouse.Web.Data;
using CraftHouse.Web.DTOs;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class OptionsManagement : PageModel
{
    private readonly IOptionRepository _optionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<Option> _optionValidator;
    private readonly IValidator<OptionValue> _optionValueValidator;

    public OptionsManagement(IOptionRepository optionRepository, IProductRepository productRepository,
        IValidator<Option> optionValidator, IValidator<OptionValue> optionValueValidator)
    {
        _optionRepository = optionRepository;
        _productRepository = productRepository;
        _optionValidator = optionValidator;
        _optionValueValidator = optionValueValidator;
    }

    [BindProperty]
    public string Name { get; set; } = null!;
    
    [BindProperty]
    public int OptionId { get; set; }
    
    [BindProperty]
    public int ProductId { get; set; }

    [BindProperty]
    public int MaxOccurs { get; set; }

    [BindProperty]
    public List<string> OptionValues { get; set; } = null!;

    [BindProperty]
    public List<float> OptionPrices { get; set; } = null!;

    public List<Option> ExistingOptions { get; set; } = null!;

    public int OptionNumber { get; set; }
    
    public List<string>? Errors { get; set; }

    public Product Product { get; set; } = null!;

    public async Task<IActionResult> OnGet(int productId, int optionNumber, CancellationToken cancellationToken)
    {
        Product = await _productRepository.GetProductByIdAsync(productId, cancellationToken);

        if (Product is null)
        {
            throw new NullReferenceException("Product does not exists");
        }

        if (optionNumber < 1)
        {
            return Redirect($"/admin/OptionsManagement/{Product.Id}?optionNumber={1}");
        }

        OptionNumber = optionNumber;
        ExistingOptions = await _optionRepository.GetOptionsByProductIdAsync(Product.Id, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostOptionAsync(CancellationToken cancellationToken)
    {
        Product = await _productRepository.GetProductByIdAsync(ProductId, cancellationToken);

        if (Product is null)
        {
            throw new NullReferenceException("Product does not exists");
        }
        
        foreach (var data in OptionValues.Select((value, i) => new {i , value}))
        {
            var optionValueDto = new OptionValueDto()
            {
                Value = data.value,
                Price = OptionPrices[data.i],
                OptionId = 1
            };
            var optionValue = optionValueDto.MapToOptionValue();
            var validationResult = await _optionValueValidator.ValidateAsync(optionValue, cancellationToken);

            if (validationResult.IsValid) continue;
            ExistingOptions = await _optionRepository.GetOptionsByProductIdAsync(Product.Id, cancellationToken);
            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            return Page();
        }

        var optionDto = new OptionDto()
        {
            Name = Name,
            MaxOccurs = MaxOccurs,
            ProductId = Product.Id
        };

        var option = optionDto.MapToOption();
        var optionValidationResult = await _optionValidator.ValidateAsync(option, cancellationToken);
        if (!optionValidationResult.IsValid)
        {
            ExistingOptions = await _optionRepository.GetOptionsByProductIdAsync(Product.Id, cancellationToken);
            Errors = optionValidationResult.Errors.Select(x => x.ErrorMessage).ToList();
            return Page();
        }

        ICollection<OptionValue> optionValues =
            OptionValues.Select((t, i) => new OptionValue() { Value = t, Price = OptionPrices[i] }).ToList();

        option.OptionValues = optionValues;

        await _optionRepository.AddOptionAsync(option, cancellationToken);

        return Redirect($"/admin/OptionsManagement/{Product.Id}?optionNumber={1}");
    }

    public async Task<IActionResult> OnPostRemoveAsync(CancellationToken cancellationToken)
    {
        ExistingOptions = await _optionRepository.GetOptionsByProductIdAsync(Product.Id, cancellationToken);

        foreach (var option in ExistingOptions)
        {
            var isDeleted = await _optionRepository.IsOptionDeletedAsync(option, cancellationToken);
            if (option.Id != OptionId || isDeleted) continue;
            await _optionRepository.DeleteOptionAsync(option, cancellationToken);
            break;
        }

        return Redirect($"/admin/OptionsManagement/{Product.Id}?optionNumber={1}");
    }
}