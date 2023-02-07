using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly AppDbContext _context;
    private readonly ILogger<Index> _logger;

    public IndexModel(IAuthService authService, AppDbContext context, ILogger<Index> logger)
    {
        _authService = authService;
        _context = context;
        _logger = logger;
    }

    public User? LoggedInUser { get; set; }
    public List<Product> Products { get; set; } = null!;

    public int PageNumber { get; set; }

    public async Task<IActionResult> OnGetAsync(int pageNumber, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Page number: {@number}", pageNumber);
        LoggedInUser = await _authService.GetLoggedInUserAsync(cancellationToken);

        if (pageNumber <= 0)
        {
            throw new InvalidOperationException("Page does not exists");
        }

        PageNumber = pageNumber;
        const int productsPerPage = 15;
        var toSkip = productsPerPage * (pageNumber - 1);

        Products = _context
            .Products
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.Id)
            .Skip(toSkip)
            .Take(productsPerPage)
            .ToList();

        var productCount = _context.Products.Count(x => x.DeletedAt == null);
        ViewData["lastPageNumber"] = 1 + productCount / productsPerPage;

        if (Products.Count == 0)
        {
            throw new InvalidOperationException("Page does not exists");
        }

        return Page();
    }
}