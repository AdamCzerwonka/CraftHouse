using CraftHouse.Web.Data;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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
    public float CartPrice { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var entries = _cartService.GetCartEntries();
        var cartProducts = new List<CartEntryProduct>();

        foreach (var entry in entries)
        {
            var product = await _context
                .Products
                .AsNoTracking()
                .Where(x => x.DeletedAt == null)
                .FirstOrDefaultAsync(x => x.Id == entry.ProductId, cancellationToken);

            if (product is null)
            {
                throw new InvalidOperationException("Product not found");
            }

            var cartProduct = new CartEntryProduct()
            {
                EntryId = entry.Id,
                Name = product.Name,
                BasePrice = product.Price,
                TotalPrice = product.Price
            };

            if (entry.Options is not null)
            {
                foreach (var option in entry.Options)
                {
                    var opt = await _context
                        .Options
                        .Include(x => x.OptionValues)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == option.OptionId, cancellationToken);

                    if (opt is null)
                    {
                        throw new InvalidOperationException("Option not found");
                    }

                    var mappedValues = option.Values.Select(valueId =>
                    {
                        var value = opt.OptionValues.First(x => x.Id == valueId);
                        return new CartOptionValue()
                        {
                            Name = value.Value,
                            Price = value.Price
                        };
                    }).ToList();

                    cartProduct.TotalPrice += mappedValues.Sum(x => x.Price);

                    var cartOption = new CartOption()
                    {
                        Name = opt.Name,
                        Values = mappedValues
                    };

                    cartProduct.Options.Add(cartOption);
                }
            }

            CartPrice += cartProduct.TotalPrice;
            cartProducts.Add(cartProduct);
        }

        CartEntries = cartProducts;
    }

    [BindProperty]
    public DeleteEntryModel DeleteEntry { get; set; } = null!;

    public IActionResult OnPostDelete()
    {
        var id = Guid.Parse(DeleteEntry.EntryId);
        _cartService.RemoveCartEntry(id);

        return Redirect("/cart");
    }
}

public record DeleteEntryModel(string EntryId);

public class CartEntryProduct
{
    public Guid EntryId { get; init; }
    public string Name { get; init; } = null!;
    public float BasePrice { get; init; }

    public float TotalPrice { get; set; }

    public List<CartOption> Options { get; } = new();
}

public class CartOption
{
    public string Name { get; init; } = null!;
    public IEnumerable<CartOptionValue> Values { get; init; } = null!;
}

public class CartOptionValue
{
    public string Name { get; init; } = null!;
    public float Price { get; init; }
}