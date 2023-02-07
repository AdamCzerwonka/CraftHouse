using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class User : PageModel
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<User> _logger;

    public User(IUserRepository userRepository, ILogger<User> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public Entities.User UserData { get; set; } = null!;

    [BindProperty]
    public UserType UserType { get; set; }

    [BindProperty]
    public int UserId { get; set; }

    public async Task<IActionResult> OnGet(int userId, CancellationToken cancellationToken)
    {
        UserId = userId;
        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        UserData = user;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(UserId, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        user.UserType = UserType;
        await _userRepository.UpdateUserAsync(user, cancellationToken);
        return Redirect("/admin/users");
    }
}