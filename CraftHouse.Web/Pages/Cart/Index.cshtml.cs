using CraftHouse.Web.Data;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace CraftHouse.Web.Pages.Cart;

public class Index : PageModel
{
    private readonly ICartService _cartService;
    private readonly AppDbContext _context;

    public Index(ICartService cartService, AppDbContext context)
    {
        _cartService = cartService;
        _context = context;
    }

    public IEnumerable<CartEntryProduct> CartEntries { get; set; } = null!;
    
    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var entries = _cartService.GetCartEntries();
        var productIds = entries.Select(x => x.ProductId);
        var prods = await _context
            .Products
            .AsNoTracking()
            .Where(x => productIds.Contains(x.Id))
            .Select(x=> new CartEntryProduct()
            {
                Name = x.Name,
                Price = x.Price
            })
            .ToListAsync(cancellationToken);

        CartEntries = prods;
    }
}

public class CartEntryProduct
{
    public string Name { get; set; } = null!;
    public float Price { get; set; }
}