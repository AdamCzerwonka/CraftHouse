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

   public UserRepository(AppDbContext context,IValidator<User> validator)
   {
      _context = context;
      _validator = validator;
   }

   public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken)
      => await _context
         .Users
         .AsNoTracking()
         .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

   public User? GetUserByEmail(string email)
   {
      var user = _context.Users.FirstOrDefault(x => x.Email == email);
      return user;
   }

   public IEnumerable<User> Get()
   {
      return _context.Users.Where(x => x.DeletedAt == null).OrderBy(x => x.Id);
   }

   public async Task<Result> CreateAsync(User user, string password)
   {
      var validationResult = await _validator.ValidateAsync(user);
      if (!validationResult.IsValid)
      {
         return new Result
         {
            Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList(),
            Succeeded = false
         };
      }
      
      var isUserAlreadyInDb = _context.Users.Any(x => x.Email == user.Email && x.DeletedAt == null);
      if (isUserAlreadyInDb)
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
      await _context.SaveChangesAsync();

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

   public async Task DeleteUserAsync(User user)
   {
      user.DeletedAt = DateTime.Now;
      _context.Users.Update(user);
      await _context.SaveChangesAsync();
   }
}