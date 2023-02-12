using System.Text.Json;

namespace CraftHouse.Web.Infrastructure;

public static class SessionExtensions
{
    public static T GetFromJson<T>(this ISession session, string key)
        where T : class, new()
    {
        var json = session.GetString(key);
        
        return json is null
            ? new T()
            : JsonSerializer.Deserialize<T>(json) ?? new T();
    }

    public static void SetAsJson<T>(this ISession session, string key, T obj)
    {
        var json = JsonSerializer.Serialize(obj);
        session.SetString(key, json);
    }
}