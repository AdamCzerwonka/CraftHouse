using System.Net;
using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

public class Users : PageModel
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<Users> _logger;
    private readonly IUserRepository _userRepository;

    public Users(AppDbContext context, IAuthService authService, ILogger<Users> logger, IUserRepository userRepository)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
        _userRepository = userRepository;
    }

    public List<Entities.User> AppUsers { get; set; } = null!;

    public IActionResult OnGet()
    {
        var user = _authService.GetLoggedInUser();
        if (user == null)
        {
            _logger.LogInformation("Location was: {@location}", HttpContext.Request.Path.Value);
            return Redirect("/login?RedirectUrl=" + HttpContext.Request.Path.Value);
        }

        if (user.UserType != UserType.Administrator)
        {
            return NotFound();
        }

        AppUsers = _context.Users.Where(x => x.DeletedAt == null).OrderBy(x => x.Id).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int userId)
    {
        var user = _userRepository.GetUserById(userId);
        if (user is null)
        {
            return NotFound();
        }

        await _userRepository.DeleteUserAsync(user);
        
        return Redirect("/admin/users");
    }
}