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
    public string FirstName { get; set; } = null!;

    [BindProperty]
    public string LastName { get; set; } = null!;

    [BindProperty]
    public string TelephoneNumber { get; set; } = null!;

    [BindProperty]
    public string City { get; set; } = null!;

    [BindProperty]
    public string PostalCode { get; set; } = null!;

    [BindProperty]
    public string AddressLine { get; set; } = null!;

    [BindProperty]
    public string Mail { get; set; } = null!;

    [BindProperty]
    public string Password { get; set; } = null!;

    [BindProperty]
    public string ConfirmPassword { get; set; } = null!;

    public List<string>? ValidationErrors { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = new User()
        {
            FirstName = FirstName,
            LastName = LastName,
            TelephoneNumber = TelephoneNumber,
            City = City,
            PostalCode = PostalCode,
            AddressLine = AddressLine,
            Email = Mail
        };

       var result =  await _authService.RegisterUser(user, Password);
       if (!result.Succeeded)
       {
           ValidationErrors = result.Errors;
           return Page();
       }

        return RedirectToPage("index");
    }
}