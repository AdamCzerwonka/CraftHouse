using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Models;

namespace CraftHouse.Web.Services;

public class CartService : ICartService
{
    private readonly ISession _session;
    private const string SessionCartKey = "cart";

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _session = httpContextAccessor.HttpContext!.Session;
    }

    public void AddCartEntry(CartEntry entry)
    {
        var cart = _session.GetFromJson<List<CartEntry>>(SessionCartKey);
        
        cart.Add(entry);

        _session.SetAsJson(SessionCartKey, cart);
    }

    public IEnumerable<CartEntry> GetCartEntries()
        => _session.GetFromJson<List<CartEntry>>(SessionCartKey);

    public void RemoveCartEntry(Guid entryId)
    {
        var cart = _session.GetFromJson<List<CartEntry>>(SessionCartKey);
        var entryToDel = cart.FirstOrDefault(x => x.Id == entryId);
        if (entryToDel is null)
        {
            return;
        }
        cart.Remove(entryToDel);
        
        _session.SetAsJson(SessionCartKey, cart);
    }
}
