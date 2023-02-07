using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;

namespace CraftHouse.Web.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    User? GetUserByEmail(string email);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task DeleteUserAsync(User user);
    IEnumerable<User> Get();
    Task<Result> CreateAsync(User user, string password);
    Task<Result> UpdateUserPasswordAsync(User user, string password, CancellationToken cancellationToken);
}