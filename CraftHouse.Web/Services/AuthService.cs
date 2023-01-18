using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using Konscious.Security.Cryptography;

namespace CraftHouse.Web.Services;

class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }


    public async Task RegisterUser(User user, string password)
    {
        //TODO: Validate User
            
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