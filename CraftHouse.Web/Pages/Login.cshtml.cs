using CraftHouse.Web.Data;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class Login : PageModel
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<Login> _logger;

    public Login(AppDbContext context, IAuthService authService, ILogger<Login> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    public string Email { get; set; } = null!;

    [BindProperty]
    public string Password { get; set; } = null!;

    [BindProperty]
    public string? ReturnUrl { get; set; }

    public void OnGet([FromQuery]string redirectUrl)
    {
        ReturnUrl = redirectUrl;
    }

    public IActionResult OnPost()
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == Email);
        if (user is null)
        {
            return RedirectToPage("login");
        }

        var result = _authService.Login(user, Password);
        if (!result)
        {
            return Page();
        }

        return Redirect(ReturnUrl ?? "/");
    }
}