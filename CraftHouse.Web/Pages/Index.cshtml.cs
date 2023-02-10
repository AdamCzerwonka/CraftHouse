using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly AppDbContext _context;

    public IndexModel(IAuthService authService, AppDbContext context)
    {
        _authService = authService;
        _context = context;
    }
    
    public List<Product> Products { get; set; } = null!;
    public int PageNumber { get; set; }

    public async Task<IActionResult> OnGetAsync(int pageNumber, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0)
        {
            throw new InvalidOperationException("Page does not exists");
        }

        PageNumber = pageNumber;
        const int productsPerPage = 15;
        var toSkip = productsPerPage * (pageNumber - 1);

        var query = _context
            .Products
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.Id);

        var productCount = await query.CountAsync(cancellationToken);

        Products = await query
            .Skip(toSkip)
            .Take(productsPerPage)
            .ToListAsync(cancellationToken);

        ViewData["lastPageNumber"] = 1 + productCount / productsPerPage;

        if (Products.Count == 0 && pageNumber > 1)
        {
            throw new InvalidOperationException("Page does not exists");
        }

        return Page();
    }
}