using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Helpers;
using CraftHouse.Web.Repositories;
using FluentValidation;
using Konscious.Security.Cryptography;

namespace CraftHouse.Web.Services;

public class AuthService : IAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;
    private readonly IUserRepository _userRepository;

    public AuthService(IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger,
        IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _userRepository = userRepository;
    }

    public bool VerifyUserPassword(User user, string password)
    {
        var salt = Convert.FromBase64String(user.PasswordSalt);
        var hash = Convert.FromBase64String(user.PasswordHash);

        return HashingHelper.VerifyHash(password, salt, hash);
    }

    public bool Login(User user, string password)
    {
        var isValidPassword = VerifyUserPassword(user, password);
        if (!isValidPassword)
        {
            return false;
        }

        _httpContextAccessor.HttpContext!.Session.SetInt32("userID", user.Id);

        return true;
    }

    public bool IsLoggedIn()
    {
        var userId = _httpContextAccessor.HttpContext!.Session.GetInt32("userID");
        return userId != null;
    }

    public async Task<User?> GetLoggedInUserAsync(CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext!.Session.GetInt32("userID");
        var user = await _userRepository.GetUserByIdAsync(userId.GetValueOrDefault(), cancellationToken);
        return userId is null ? null : user;
    }

    public void Logout()
    {
        _httpContextAccessor.HttpContext!.Session.Clear();
    }
}

public class Result
{
    public bool Succeeded { get; init; }
    public List<string> Errors { get; init; } = null!;
}