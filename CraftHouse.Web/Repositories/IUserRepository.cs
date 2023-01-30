using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;

namespace CraftHouse.Web.Repositories;

public interface IUserRepository
{
    User? GetUserById(int id);
    User? GetUserByEmail(string email);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
    IEnumerable<User> Get();
    Task<Result> CreateAsync(User user, string password);
}