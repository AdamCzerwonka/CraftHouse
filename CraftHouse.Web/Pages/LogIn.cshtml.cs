using CraftHouse.Web.Data;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class LogIn : PageModel
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;

    public LogIn(AppDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [BindProperty]
    public string Email { get; set; } = null!;

    [BindProperty]
    public string Password { get; set; } = null!;

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == Email);
        if (user is null)
        {
            return RedirectToPage("login");
        }

        var result = _authService.Login(user, Password);
        return RedirectToPage(result ? "index" : "login");
    }
}