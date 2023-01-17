using System.ComponentModel.DataAnnotations;
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
    public string LastName { get; set; }

    [BindProperty]
    public string TelephoneNumber { get; set; }

    [BindProperty]
    public string City { get; set; }

    [BindProperty]
    public string PostalCode { get; set; }

    [BindProperty]
    public string AddressLine { get; set; }

    [BindProperty]
    public string Mail { get; set; }

    [BindProperty]
    public string Password { get; set; }

    [BindProperty]
    public string ConfirmPassword { get; set; }

    public void OnGet()
    {
    }

    public async Task OnPostAsync()
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

        await _authService.RegisterUser(user, Password);
    }
}