using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class Login : PageModel
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public Login(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    [BindProperty]
    public LoginUserModel LoginUser { get; set; } = new();

    public string? Error { get; set; }

    public void OnGet([FromQuery] string redirectUrl)
    {
        LoginUser.ReturnUrl = redirectUrl;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(LoginUser.Email, cancellationToken);
        if (user is null)
        {
            Error = "Provided email and password combination is invalid. Try again!";
            return Page();
        }

        var result = _authService.Login(user, LoginUser.Password);
        if (result)
        {
            return Redirect(LoginUser.ReturnUrl ?? "/");
        }

        Error = "Provided email and password combination is invalid. Try again!";
        return Page();
    }
}

public class LoginUserModel
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string? ReturnUrl { get; set; }
};