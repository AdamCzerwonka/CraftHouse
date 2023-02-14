using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly AppDbContext _context;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IAuthService authService, AppDbContext context, ICategoryRepository categoryRepository, ILogger<IndexModel> logger)
    {
        _authService = authService;
        _context = context;
        _categoryRepository = categoryRepository;
        _logger = logger;
    }

    [BindProperty]
    public int? CategoryId { get; set; }
    public List<Category> Categories { get; set; } = null!;
    public List<Product> Products { get; set; } = null!;
    public int PageNumber { get; set; }

    public async Task<IActionResult> OnGetAsync(int pageNumber, int? category, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0)
        {
            throw new InvalidOperationException("Page does not exists");
        }

        CategoryId = category;
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
        PageNumber = pageNumber;
        const int productsPerPage = 15;
        var toSkip = productsPerPage * (pageNumber - 1);

        var query = _context
            .Products
            .AsNoTracking()
            .Where(x => x.DeletedAt == null);

        if (category is not null)
        {
            query = query
                .Where(x => x.CategoryId == category);
        }

        var productCount = await query.CountAsync(cancellationToken);

        Products = await query
            .OrderBy(x=>x.Id)
            .Skip(toSkip)
            .Take(productsPerPage)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        ViewData["lastPageNumber"] = 1 + productCount / productsPerPage;

        if (Products.Count == 0 && pageNumber > 1)
        {
            throw new InvalidOperationException("Page does not exists");
        }

        return Page();
    }
}