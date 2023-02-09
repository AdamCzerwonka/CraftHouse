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

    public async Task OnGet(int productId, CancellationToken cancellationToken)
    {
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
        ProductId = productId;
    }

    public async Task<IActionResult> OnPostEditAsync(CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductByIdAsync(ProductId, cancellationToken);
        var category = await _categoryRepository.GetCategoryByIdAsync(CategoryId, cancellationToken);

        product!.Name = Name;
        product.IsAvailable = IsAvailable;
        product.Price = Price;
        product.Description = Description;
        product.CategoryId = category!.Id;

        await _productRepository.UpdateProductAsync(product, cancellationToken);

        return Redirect($"/admin/ProductPage/{ProductId}");
    }
}