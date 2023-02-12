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
public class ProductManagement : PageModel
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<Product> _validator;

    public ProductManagement(ICategoryRepository categoryRepository, IProductRepository productRepository,
        IValidator<Product> validator)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _validator = validator;
    }

    public List<Category> Categories { get; set; } = null!;

    [BindProperty]
    public int ProductId { get; set; }

    [BindProperty]
    public ProductDto ProductDto { get; set; } = null!;

    public List<Product> Products { get; set; } = null!;

    public List<string>? Errors { get; set; }

    public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
    {
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
        Products = await _productRepository.GetProductsAsync(cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostProductAsync(CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(ProductDto.CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NullReferenceException("Category does not exists");
        }

        var product = ProductDto.MapToProduct();
        var validationResult = await _validator.ValidateAsync(product, cancellationToken);

        if (!validationResult.IsValid)
        {
            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
            Products = await _productRepository.GetProductsAsync(cancellationToken);
            return Page();
        }

        await _productRepository.AddProductAsync(product, cancellationToken);

        return Redirect("/admin/Products");
    }

    public async Task<RedirectResult> OnPostRemoveAsync(CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(ProductId, cancellationToken);

        if (product is null)
        {
            throw new NullReferenceException("Product does not exists");
        }

        await _productRepository.DeleteProductWithOptionsAsync(product, cancellationToken);

        return Redirect("/admin/Products");
    }
}