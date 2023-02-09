using CraftHouse.Web.DTOs;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class Registration : PageModel
{
    private readonly IUserRepository _userRepository;

    public Registration(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [BindProperty]
    public RegisterUserDto UserRegister { get; set; } = null!;

    public List<string>? ValidationErrors { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var user = UserRegister.MapToUser();

        if (UserRegister.Password != UserRegister.ConfirmPassword)
        {
            ValidationErrors = new List<string> { "Passwords do not match" };
        }

        var result = await _userRepository.CreateUserAsync(user, UserRegister.Password, cancellationToken);
        if (result.Succeeded)
        {
            return RedirectToPage("/Index");
        }
        ValidationErrors = result.Errors;
        return Page();
    }
}