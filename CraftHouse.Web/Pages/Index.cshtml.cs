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

    public IndexModel(ILogger<IndexModel> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    public User? LoggedInUser { get; set; }

    public void OnGet()
    {
        LoggedInUser = _authService.GetLoggedInUser();
    }
}