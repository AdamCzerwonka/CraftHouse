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

    public void OnGet()
    {
        LoggedInUser = _authService.GetLoggedInUser();
        Products = _context.Products.ToList();
    }
}