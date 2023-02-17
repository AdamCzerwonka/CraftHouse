using CraftHouse.Web.Entities;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public IndexModel(ICategoryRepository categoryRepository,
        ILogger<IndexModel> logger, IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public int? CategoryId { get; set; }
    public int PageNumber { get; set; }
    public string? Order { get; set; }
    public bool? IsAscending { get; set; }
    public List<Category> Categories { get; set; } = null!;
    public List<Product> Products { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int pageNumber, int? category, string? order, bool? isAscending,
        CancellationToken cancellationToken)
    {
        if (pageNumber <= 0)
        {
            throw new InvalidOperationException("Page does not exists");
        }

        CategoryId = category;
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
        PageNumber = pageNumber;
        Order = order;
        IsAscending = isAscending;
        const int productsPerPage = 15;

        var productCount = await _productRepository.GetProductsCountByCategoryAsync(CategoryId, cancellationToken);

        Products = await _productRepository
            .GetProductsByCategoryWithSortingAsync(
                pageNumber,
                CategoryId,
                Order,
                IsAscending,
                cancellationToken);

        ViewData["lastPageNumber"] = 1 + productCount / productsPerPage;

        if (Products.Count == 0 && pageNumber > 1)
        {
            throw new InvalidOperationException("Page does not exists");
        }

        return Page();
    }
}