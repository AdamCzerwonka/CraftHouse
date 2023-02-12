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
        var cartProducts = new List<CartEntryProduct>();

        foreach (var entry in entries)
        {
            var cartProduct = new CartEntryProduct();

            var product = await _context
                .Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == entry.ProductId && x.DeletedAt == null, cancellationToken);

            if (entry.Options is not null)
            {
                foreach (var option in entry.Options)
                {
                    var opt = await _context
                        .Options
                        .Include(x=>x.OptionValues)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == option.OptionId, cancellationToken);


                    var cartOption = new CartOption()
                    {
                        Name = opt!.Name
                    };
                    
                    foreach (var value in option.Values)
                    {
                        var val = opt.OptionValues.FirstOrDefault(x => x.Id == value);

                        var optionValue = new CartOptionValue()
                        {
                            Name = val.Value,
                            Price = val.Price
                        };
                        
                        cartOption.Values.Add(optionValue);
                    }

                    cartProduct.Options.Add(cartOption);
                }
            }

            cartProduct.Name = product!.Name;
            cartProduct.Price = product!.Price;

            cartProducts.Add(cartProduct);
        }

        CartEntries = cartProducts;
    }
}

public class CartEntryProduct
{
    public string Name { get; set; } = null!;
    public float Price { get; set; }

    public List<CartOption> Options { get; set; } = new();
}

public class CartOption
{
    public string Name { get; set; } = null!;
    public List<CartOptionValue> Values { get; set; } = new();
}

public class CartOptionValue
{
    public string Name { get; set; } = null!;
    public float Price { get; set; }
}
