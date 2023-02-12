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
    public int CategoryId { get; set; } = 0;
    public List<Category> Categories { get; set; } = null!;
    public List<Product> Products { get; set; } = null!;
    public int PageNumber { get; set; }

    public async Task<IActionResult> OnGetAsync(int pageNumber, int categoryId, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0)
        {
            throw new InvalidOperationException("Page does not exists");
        }

        CategoryId = categoryId;
        Categories = await _categoryRepository.GetCategoriesAsync(cancellationToken);
        PageNumber = pageNumber;
        const int productsPerPage = 15;
        var toSkip = productsPerPage * (pageNumber - 1);

        IOrderedQueryable<Product> query;

        _logger.LogInformation(CategoryId.ToString());
        
        if (CategoryId != 0)
        {
            query = _context
                .Products
                .Where(x => x.DeletedAt == null && x.CategoryId == CategoryId)
                .OrderBy(x => x.Id);
        }
        else
        {
            query = _context
                .Products
                .Where(x => x.DeletedAt == null)
                .AsNoTracking()
                .OrderBy(x => x.Id);
        }
        var productCount = await query.CountAsync(cancellationToken);

        Products = await query
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

    public async Task<IActionResult> OnPostCategoryAsync()
    {
        return Redirect($"/1?CategoryId={CategoryId}");
    }
}