using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class Registration : PageModel
{
    [BindProperty]
    public string FirstName { get; set; }

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

    public void OnPost()
    {
    }
}