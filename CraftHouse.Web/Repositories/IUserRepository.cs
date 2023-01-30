using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface IUserRepository
{
    User? GetUserById(int id);
    User? GetUserByEmail(string email);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
    IEnumerable<User> Get();
}