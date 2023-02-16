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
        var cart = GetEntriesFromSession();
        
        cart.Add(entry);

        _session.SetAsJson(SessionCartKey, cart);
    }

    public IEnumerable<CartEntry> GetCartEntries()
        => GetEntriesFromSession();

    public void RemoveCartEntry(Guid entryId)
    {
        var cart = GetEntriesFromSession();
        var entryToDel = cart.FirstOrDefault(x => x.Id == entryId);
        if (entryToDel is null)
        {
            return;
        }
        cart.Remove(entryToDel);
        
        _session.SetAsJson(SessionCartKey, cart);
    }

    public void ClearCart()
    {
        _session.Remove(SessionCartKey);
    }

    private List<CartEntry> GetEntriesFromSession()
        => _session.GetFromJson<List<CartEntry>>(SessionCartKey);
}
