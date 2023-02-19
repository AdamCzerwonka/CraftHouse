using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Account;

[RequireAuth]
public class IndexModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;

    public IndexModel(IAuthService authService, IUserRepository userRepository, IOrderRepository orderRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
        _orderRepository = orderRepository;
    }

    [BindProperty]
    public UpdatePasswordModel UpdatePassword { get; set; } = null!;
    
    public User UserData { get; set; } = null!;

    public List<string> Errors { get; set; } = new();

    public  List<Order> Orders { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        UserData = (await _authService.GetLoggedInUserAsync(cancellationToken))!;
        Orders = (await _orderRepository.GetOrdersByUserAsync(UserData.Id, cancellationToken));
    }

    public async Task<IActionResult> OnPostPasswordAsync(CancellationToken cancellationToken)
    {
        UserData = (await _authService.GetLoggedInUserAsync(cancellationToken))!;

        var passwordCorrect = _authService.VerifyUserPassword(UserData, UpdatePassword.OldPassword);
        if (!passwordCorrect)
        {
            Errors.Add("Wrong password");
            return Page();
        }

        if (UpdatePassword.Password != UpdatePassword.Password2)
        {
            Errors.Add("Passwords are not the same");
            return Page();
        }

        await _userRepository.UpdateUserPasswordAsync(UserData, UpdatePassword.Password, cancellationToken);
        return Redirect("/account");
    }
}

public record UpdatePasswordModel(string OldPassword, string Password, string Password2);