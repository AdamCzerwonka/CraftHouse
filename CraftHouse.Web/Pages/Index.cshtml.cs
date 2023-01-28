using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace CraftHouse.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly IAuthService _authService;
    private readonly AppDbContext _context;

    public IndexModel(ILogger<IndexModel> logger, IAuthService authService, AppDbContext context)
    {
        _logger = logger;
        _authService = authService;
        _context = context;
    }

    public User? LoggedInUser { get; set; }
    public List<Product> Products { get; set; } = null!;

    public int PageNumber { get; set; }

    public IActionResult OnGet(int pageNumber = 1)
    {
        if (pageNumber == 0)
        {
            pageNumber = 1;
        }

        PageNumber = pageNumber;
        const int productsPerPage = 15;
        var toSkip = productsPerPage * (pageNumber - 1);
        
        LoggedInUser = _authService.GetLoggedInUser();
        Products = _context.Products.Where(x => x.DeletedAt == null).Skip(toSkip).Take(productsPerPage).ToList();
        if (Products.Count == 0)
        {
            return Redirect($"/index/{PageNumber - 1}");
        }

        return Page();
    }
}