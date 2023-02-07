using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class Users : PageModel
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public Users(AppDbContext context, IAuthService authService, IUserRepository userRepository)
    {
        _context = context;
        _authService = authService;
        _userRepository = userRepository;
    }

    public List<Entities.User> AppUsers { get; set; } = null!;

    public IActionResult OnGet()
    {
        AppUsers = _context.Users.Where(x => x.DeletedAt == null).OrderBy(x => x.Id).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int userId, CancellationToken cancellationToken)
    {
        var loggedInUser = await _authService.GetLoggedInUserAsync(cancellationToken);
        if (loggedInUser!.Id  == userId)
        {
            throw new InvalidOperationException("Cannot delete currently logged in user");
        }

        var user = await _userRepository.GetUserByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        await _userRepository.DeleteUserAsync(user, cancellationToken);

        return Redirect("/admin/users");
    }
}