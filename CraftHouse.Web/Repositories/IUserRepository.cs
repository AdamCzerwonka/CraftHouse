using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;

namespace CraftHouse.Web.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task DeleteUserAsync(User user, CancellationToken cancellationToken);
    Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<Result> CreateUserAsync(User user, string password, CancellationToken cancellationToken);
    Task<Result> UpdateUserPasswordAsync(User user, string password, CancellationToken cancellationToken);
}