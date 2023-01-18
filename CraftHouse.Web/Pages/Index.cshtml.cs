using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    public string UserId { get; set; } = null!;

    public void OnGet()
    {
        var user = _authService.GetLoggedInUser();
        if(user is not null)
        {
            UserId = user.Id.ToString();
        }
    }
}