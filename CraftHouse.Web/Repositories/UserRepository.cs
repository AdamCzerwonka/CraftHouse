using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Helpers;
using CraftHouse.Web.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly IValidator<User> _validator;

    public UserRepository(AppDbContext context, IValidator<User> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
        => await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        => await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email && x.DeletedAt == null, cancellationToken);

    public Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        => _context.Users.OrderBy(x => x.Id).ToListAsync(cancellationToken);


    public async Task<Result> CreateUserAsync(User user, string password, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(user, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new Result
            {
                Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList(),
                Succeeded = false
            };
        }

        var isUserAlreadyInDb = await GetUserByEmailAsync(user.Email, cancellationToken);
        if (isUserAlreadyInDb is not null)
        {
            return new Result
            {
                Errors = new List<string> { "User with given email already exists." },
                Succeeded = false
            };
        }

        var passwordSalt = HashingHelper.CreateSalt();
        var passwordHash = HashingHelper.HashPassword(password, passwordSalt);

        user.PasswordHash = Convert.ToBase64String(passwordHash);
        user.PasswordSalt = Convert.ToBase64String(passwordSalt);

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new Result { Succeeded = true };
    }

    public async Task<Result> UpdateUserPasswordAsync(User user, string password, CancellationToken cancellationToken)
    {
        var passwordSalt = HashingHelper.CreateSalt();
        var passwordHash = HashingHelper.HashPassword(password, passwordSalt);

        user.PasswordHash = Convert.ToBase64String(passwordHash);
        user.PasswordSalt = Convert.ToBase64String(passwordSalt);

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new Result { Succeeded = true };
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        user.UpdatedAt = DateTime.Now;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteUserAsync(User user, CancellationToken cancellationToken)
    {
        user.DeletedAt = DateTime.Now;
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}