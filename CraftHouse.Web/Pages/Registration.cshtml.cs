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
    public UserRegisterModel UserRegister { get; set; } = null!;

    public List<string>? ValidationErrors { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = new User()
        {
            FirstName = UserRegister.FirstName,
            LastName = UserRegister.LastName,
            TelephoneNumber = UserRegister.TelephoneNumber,
            City = UserRegister.City,
            PostalCode = UserRegister.PostalCode,
            AddressLine = UserRegister.AddressLine,
            Email = UserRegister.Mail
        };

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

public class UserRegisterModel
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string TelephoneNumber { get; init; } = null!;
    public string City { get; init; } = null!;
    public string PostalCode { get; init; } = null!;
    public string AddressLine { get; init; } = null!;
    public string Mail { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string ConfirmPassword { get; init; } = null!;
}