using CraftHouse.Web.DTOs;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class Registration : PageModel
{
    private readonly IAuthService _authService;

    public Registration(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public RegisterUserDto UserRegister { get; set; } = null!;

    public List<string>? ValidationErrors { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = UserRegister.MapToUser();

        if (UserRegister.Password != UserRegister.ConfirmPassword)
        {
            ValidationErrors = new List<string> { "Passwords do not match" };
        }

        var result = await _authService.RegisterUser(user, UserRegister.Password);
        if (!result.Succeeded)
        {
            ValidationErrors = result.Errors;
            return Page();
        }

        return RedirectToPage("index");
    }
}
