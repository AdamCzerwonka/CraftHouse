using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace CraftHouse.Web.Services;

public class CartService : ICartService
{
    private readonly ILogger<CartService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string SessionCartKey = "cart";

    public CartService(ILogger<CartService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public void AddCartEntry(CartEntry entry)
    {
        var sessionCart = _httpContextAccessor.HttpContext!.Session.GetString(SessionCartKey);
        var cart =
            sessionCart is null
                ? new List<CartEntry>()
                : JsonSerializer.Deserialize<List<CartEntry>>(sessionCart);

        cart ??= new List<CartEntry>();
        
        cart.Add(entry);

        var serializedCart =  JsonSerializer.Serialize(cart);
        _logger.LogInformation("Serialized cart: {@cart}", serializedCart);
        _httpContextAccessor.HttpContext.Session.SetString(SessionCartKey, serializedCart);
    }

    public IEnumerable<CartEntry> GetCartEntries()
    {
        var sessionCart = _httpContextAccessor.HttpContext!.Session.GetString(SessionCartKey);
        var cart =
            sessionCart is null
                ? new List<CartEntry>()
                : JsonSerializer.Deserialize<List<CartEntry>>(sessionCart);
        
        return cart ?? new List<CartEntry>();
    }
}

public class CartEntry
{
    public int ProductId { get; set; }
    public IEnumerable<CartEntryOption>? Options { get; set; } = null!;
}

public class CartEntryOption
{
    public int OptionId { get; set; }
    public List<int> Options { get; } = new();
}