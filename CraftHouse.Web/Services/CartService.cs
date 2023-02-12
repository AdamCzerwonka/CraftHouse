using CraftHouse.Web.Infrastructure;

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
}

public class CartEntry
{
    public int ProductId { get; init; }
    public IEnumerable<CartEntryOption>? Options { get; set; }
}

public class CartEntryOption
{
    public int OptionId { get; set; }
    public List<int> Options { get; set; }
}