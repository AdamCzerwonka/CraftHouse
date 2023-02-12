using System.Security.Cryptography.Xml;
using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Models;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;

namespace CraftHouse.Web.Pages.Cart;

public class Add : PageModel
{
    private readonly IProductRepository _productRepository;
    private readonly ICartService _cartService;
    private readonly AppDbContext _context;
    private readonly ILogger<Add> _logger;

    public Add(AppDbContext context, ILogger<Add> logger, IProductRepository productRepository, ICartService cartService)
    {
        _productRepository = productRepository;
        _cartService = cartService;
        _context = context;
        _logger = logger;
    }

    public Product Product { get; set; } = null!;

    public List<Option> Options { get; set; } = null!;
    
    public async Task OnGetAsync(int productId, CancellationToken cancellationToken)
    {
        var product = await _context
            .Products
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == productId && x.DeletedAt == null, cancellationToken);

        Options = await _context
            .Options
            .Include(x=>x.OptionValues)
            .Where(x => x.ProductId == product!.Id)
            .ToListAsync(cancellationToken);

        Product = product ?? throw new InvalidOperationException("Product not found");
    }

    [BindProperty]
    public AddToCartModel AddToCart { get; set; }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Options: {@options}", AddToCart);
        
        // get and validate the product
        var product = await _productRepository.GetProductByIdAsync(AddToCart.ProductId, cancellationToken);
        if (product is null)
        {
            throw new InvalidOperationException("Product does not exists");
        }
        
        // validate option list

        var optionsInDb = await _context
            .Options
            .Include(x=>x.OptionValues)
            .AsNoTracking()
            .Where(x => x.ProductId == AddToCart.ProductId && x.DeletedAt == null)
            .ToListAsync(cancellationToken);

        List<CartEntryOption>? cartOptions = null;

        if (optionsInDb.Count != 0)
        {
            cartOptions = new List<CartEntryOption>();
            foreach (var opt in AddToCart.Options)
            {
                var option = optionsInDb.FirstOrDefault(x => x.Id == opt.OptionId);
                if (option is null)
                {
                    throw new InvalidOperationException("Option does not exits");
                }

                var valuesCount = opt.Values.Count;

                if (option.Required && valuesCount == 0)
                {
                    _logger.LogInformation("Option required but value was not specified");
                    throw new InvalidOperationException("Option required");
                }

                if (option.MaxOccurs < valuesCount)
                {
                    _logger.LogError("Option max occurrences were exceeded");
                    throw new InvalidOperationException("Too much values");
                }

                var cartOption = new CartEntryOption()
                {
                    OptionId = option.Id
                };

                foreach (var val in opt.Values.Select(value => option.OptionValues.FirstOrDefault(x => x.Id == value)))
                {
                    if (val is null)
                    {
                        throw new InvalidOperationException("OptionValue does not exits");
                    }

                    cartOption.Options.Add(val.Id);
                }
                
                cartOptions.Add(cartOption);
            }
        }

        var entry = new CartEntry()
        {
            ProductId = AddToCart.ProductId,
            Options = cartOptions
        };
        
        _cartService.AddCartEntry(entry);

        return Redirect("/cart");
    }
}

public class AddToCartModel
{
    public int ProductId { get; set; }
    public List<CartOptionValueModel> Options { get; set; } = null!;
}

public class CartOptionValueModel
{
    public int OptionId { get; set; }
    public List<int> Values { get; set; } = null!;
}
