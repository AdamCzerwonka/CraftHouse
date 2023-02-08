using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class ProductManagement : PageModel
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public ProductManagement(ICategoryRepository categoryRepository, IProductRepository productRepository)
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

    public List<Category> Categories { get; set; } = null!;

    [BindProperty]
    public int ProductId { get; set; }

    public List<Product> Products { get; set; } = null!;

    public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
    {
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
        Products = await _productRepository.GetProductsAsync(cancellationToken);

        return Page();
    }

    public async Task<RedirectResult> OnPostProductAsync(CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetCategoryByIdAsync(CategoryId, cancellationToken);

        if (category is null)
        {
            throw new NullReferenceException("Category does not exists");
        }

        var product = new Product()
        {
            Name = Name,
            IsAvailable = IsAvailable,
            Price = Price,
            Description = Description,
            CategoryId = category.Id
        };

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
