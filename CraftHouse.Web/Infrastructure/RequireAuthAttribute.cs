using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Infrastructure;

public class RequireAuthAttribute : Attribute
{
    public IEnumerable<UserType> AllowedTypes { get; }

    public RequireAuthAttribute(params UserType[] allowedTypes)
    {
        AllowedTypes = allowedTypes.ToList();
        if (!AllowedTypes.Any())
        {
            AllowedTypes = new List<UserType>() { UserType.Standard, UserType.Worker, UserType.Administrator };
        }
    }
}