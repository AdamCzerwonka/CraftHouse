using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class ProductPage : PageModel
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public ProductPage(ICategoryRepository categoryRepository, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
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

    public Product Product { get; set; }

    public async Task OnGet(int productId, CancellationToken cancellationToken)
    {
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
        ProductId = productId;
        Product = await _productRepository.GetProductByIdAsync(ProductId, cancellationToken);

        if (Product is null)
        {
            throw new NullReferenceException("Product does not exists");
        }
    }

    public async Task<IActionResult> OnPostEditAsync(CancellationToken cancellationToken)
    {
        Product = await _productRepository.GetProductByIdAsync(ProductId, cancellationToken);
        var category = await _categoryRepository.GetCategoryByIdAsync(CategoryId, cancellationToken);
        
        if (Product is null)
        {
            throw new NullReferenceException("Product does not exists");
        }

        if (category is null)
        {
            throw new NullReferenceException("Category does not exists");
        }

        Product!.Name = Name;
        Product.IsAvailable = IsAvailable;
        Product.Price = Price;
        Product.Description = Description;
        Product.CategoryId = category!.Id;

        await _productRepository.UpdateProductAsync(Product, cancellationToken);

        return Redirect($"/admin/ProductPage/{ProductId}");
    }
}