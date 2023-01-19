using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class Logout : PageModel
{
    private readonly IAuthService _authService;

    public Logout(IAuthService authService)
    {
        _authService = authService;
    }
    public IActionResult OnGet()
    {
       _authService.Logout();
       return RedirectToPage("index");
    }
}