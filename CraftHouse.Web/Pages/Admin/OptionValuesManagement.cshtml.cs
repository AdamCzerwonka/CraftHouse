using CraftHouse.Web.DTOs;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class OptionValuesManagement : PageModel
{
    private readonly IOptionValueRepository _optionValueRepository;
    private readonly IOptionRepository _optionRepository;
    private readonly IValidator<OptionValue> _optionValueValidator;
    private readonly IValidator<Option> _optionValidator;

    public OptionValuesManagement(IOptionValueRepository optionValueRepository, IOptionRepository optionRepository,
        IValidator<OptionValue> optionValueValidator, IValidator<Option> optionValidator)
    {
        _optionValueRepository = optionValueRepository;
        _optionRepository = optionRepository;
        _optionValueValidator = optionValueValidator;
        _optionValidator = optionValidator;
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

    [BindProperty]
    public OptionValueDto OptionValueDto { get; set; } = null!;

    public OptionDto OptionDto { get; set; } = null!;

    public List<OptionValue> OptionValues { get; set; } = null!;

    public Option Option { get; set; } = null!;

    public List<string>? Errors { get; set; }

    public async Task OnGet(int optionId, CancellationToken cancellationToken)
    {
        OptionId = optionId;
        var option = await _optionRepository.GetOptionByIdAsync(OptionId, cancellationToken);

        Option = option ?? throw new NullReferenceException("Option does not exists");

        OptionValues = await _optionValueRepository.GetOptionValuesByOptionIdAsync(OptionId, cancellationToken);
        OptionName = Option.Name;
    }

    public async Task<IActionResult> OnPostRemoveAsync(CancellationToken cancellationToken)
    {
        var optionValueToDelete =
            await _optionValueRepository.GetOptionValueAsync(OptionId, OptionValue, cancellationToken);

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

        OptionDto = new OptionDto
        {
            Name = OptionName,
            MaxOccurs = option.MaxOccurs,
            ProductId = option.ProductId
        };
        var toCheck = OptionDto.MapToOption();
        var validationResult = await _optionValidator.ValidateAsync(toCheck, cancellationToken);

        if (!validationResult.IsValid)
        {
            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            OptionValues = await _optionValueRepository.GetOptionValuesByOptionIdAsync(OptionId, cancellationToken);
            Option = option;
            return Page();
        }

        option.Name = OptionName;
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

        OptionValueDto.OptionId = OptionId;
        var optionValue = OptionValueDto.MapToOptionValue();
        var validationResult = await _optionValueValidator.ValidateAsync(optionValue, cancellationToken);

        if (!validationResult.IsValid)
        {
            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            OptionValues = await _optionValueRepository.GetOptionValuesByOptionIdAsync(OptionId, cancellationToken);
            Option = option;
            return Page();
        }

        await _optionValueRepository.AddNewOptionValueAsync(optionValue, cancellationToken);

        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }

    public async Task<IActionResult> OnPostUpdateValueAsync(CancellationToken cancellationToken)
    {
        var value = await _optionValueRepository.GetOptionValueAsync(OptionId, OldOptionValue, cancellationToken);

        if (value is null)
        {
            throw new NullReferenceException("Option value does not exists");
        }

        OptionValueDto.Value = OptionValue;
        OptionValueDto.Price = OptionPrice;
        OptionValueDto.OptionId = OptionId;
        var optionValue = OptionValueDto.MapToOptionValue();
        var validationResult = await _optionValueValidator.ValidateAsync(optionValue, cancellationToken);

        if (!validationResult.IsValid)
        {
            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            OptionValues = await _optionValueRepository.GetOptionValuesByOptionIdAsync(OptionId, cancellationToken);
            Option = await _optionRepository.GetOptionByIdAsync(OptionId, cancellationToken) ??
                     throw new InvalidOperationException("Option does not exists");
            OptionValueDto.Value = "";
            OptionValueDto.Price = 0;
            return Page();
        }

        await _optionValueRepository.UpdateOptionValueFieldsAsync(OptionId, OldOptionValue, OptionValue, OptionPrice,
            cancellationToken);

        return Redirect($"/admin/OptionValuesManagement/{OptionId}");
    }
}