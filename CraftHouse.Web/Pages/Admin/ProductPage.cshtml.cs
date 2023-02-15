using CraftHouse.Web.DTOs;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class ProductPage : PageModel
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IValidator<Product> _validator;

    public ProductPage(ICategoryRepository categoryRepository, IProductRepository productRepository,
        IValidator<Product> validator)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _validator = validator;
    }

    [BindProperty]
    public ProductDto ProductDto { get; set; } = null!;

    [BindProperty]
    public int ProductId { get; set; }

    public List<Category> Categories { get; set; } = null!;

    public Product Product { get; set; }
    
    public List<string>? Errors { get; set; }

    public async Task OnGet(int productId, CancellationToken cancellationToken)
    {
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
        ProductId = productId;
        Product = await _productRepository.GetProductByIdAsync(ProductId, cancellationToken);

        if (Product is null)
        {
            throw new NullReferenceException("Product does not exists");
        }

        ProductDto = new ProductDto
        {
            Description = Product.Description
        };
    }

    public async Task<IActionResult> OnPostEditAsync(CancellationToken cancellationToken)
    {
        Product = await _productRepository.GetProductByIdAsync(ProductId, cancellationToken);
        var category = await _categoryRepository.GetCategoryByIdAsync(ProductDto.CategoryId, cancellationToken);

        if (Product is null)
        {
            throw new NullReferenceException("Product does not exists");
        }

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
            return Page();
        }

        Product!.Name = ProductDto.Name;
        Product.IsAvailable = ProductDto.IsAvailable;
        Product.Price = ProductDto.Price;
        Product.Description = ProductDto.Description;
        Product.CategoryId = category!.Id;

        await _productRepository.UpdateProductAsync(Product, cancellationToken);

        return Redirect($"/admin/ProductPage/{ProductId}");
    }
}