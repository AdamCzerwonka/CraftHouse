using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using FluentValidation;
using Konscious.Security.Cryptography;

namespace CraftHouse.Web.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;
    private readonly IValidator<User> _validator;

    public AuthService(AppDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger,
        IValidator<User> validator)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _validator = validator;
    }


    public async Task<AuthResult> RegisterUser(User user, string password)
    {
        var result = await _validator.ValidateAsync(user);
        if (!result.IsValid)
        {
            var errors = new List<string>();
            foreach (var validationFailure in result.Errors)
            {
                errors.Add(validationFailure.ErrorMessage);
                _logger.LogWarning("Validation error: {@Error}", validationFailure.ErrorMessage);
            }



            return new AuthResult() { Succeeded = false, Errors = errors };
        }

        var userInDb = _context.Users.FirstOrDefault(x => x.Email == user.Email);
        if (userInDb is not null)
        {
            throw new InvalidOperationException("User with given email already exists in db");
        }

        var salt = CreateSalt();
        var hash = HashPassword(password, salt);

        var saltB64 = Convert.ToBase64String(salt);
        var hashB64 = Convert.ToBase64String(hash);

        user.PasswordHash = hashB64;
        user.PasswordSalt = saltB64;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new AuthResult() { Succeeded = true };
    }

    public bool VerifyUserPassword(User user, string password)
    {
        var salt = Convert.FromBase64String(user.PasswordSalt);
        var hash = Convert.FromBase64String(user.PasswordHash);

        return VerifyHash(password, salt, hash);
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

    public User? GetLoggedInUser()
    {
        var userId = _httpContextAccessor.HttpContext!.Session.GetInt32("userID");
        return userId is null ? null : _context.Users.FirstOrDefault(x => x.Id == userId);
    }

    public void Logout()
    {
        _httpContextAccessor.HttpContext!.Session.Clear();
    }

    private byte[] CreateSalt()
    {
        var buff = RandomNumberGenerator.GetBytes(16);
        return buff;
    }

    private byte[] HashPassword(string password, byte[] salt)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));

        argon2.Salt = salt;
        argon2.DegreeOfParallelism = 8;
        argon2.Iterations = 4;
        argon2.MemorySize = 1024 * 256;

        return argon2.GetBytes(16);
    }

    private bool VerifyHash(string password, byte[] salt, byte[] hash)
    {
        var newHash = HashPassword(password, salt);
        return hash.SequenceEqual(newHash);
    }
}

public class AuthResult
{
    public bool Succeeded { get; set; }
    public List<string> Errors { get; set; } = null!;
}