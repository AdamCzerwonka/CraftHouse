using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface IUserRepository
{
    User? GetUserById(int id);
    User? GetUserByEmail(string email);
}