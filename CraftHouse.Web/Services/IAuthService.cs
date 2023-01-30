using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Services;

public interface IAuthService
{
    bool VerifyUserPassword(User user, string password);
    bool Login(User user, string password);
    bool IsLoggedIn();
    User? GetLoggedInUser();
    void Logout();
}