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

    public IndexModel(IAuthService authService, AppDbContext context)
    {
        _authService = authService;
        _context = context;
    }

    public User? LoggedInUser { get; set; }
    public List<Product> Products { get; set; } = null!;

    public int PageNumber { get; set; }

    public IActionResult OnGet(int pageNumber = 1)
    {
        LoggedInUser = _authService.GetLoggedInUser();

        pageNumber = pageNumber == 0 ? 1 : pageNumber;

        PageNumber = pageNumber;
        const int productsPerPage = 15;
        var toSkip = productsPerPage * (pageNumber - 1);
        
        Products = _context
            .Products
            .Where(x => x.DeletedAt == null)
            .OrderBy(x=>x.Id)
            .Skip(toSkip)
            .Take(productsPerPage)
            .ToList();
        
        if (Products.Count == 0)
        {
            return Redirect($"/index/{PageNumber - 1}");
        }

        return Page();
    }
}