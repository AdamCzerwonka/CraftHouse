using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

[RequireAuth]
public class OrderModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ICartService _cartService;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderModel> _logger;

    public OrderModel(IAuthService authService, ICartService cartService,
        IOrderRepository orderRepository, ILogger<OrderModel> logger)
    {
        _authService = authService;
        _cartService = cartService;
        _orderRepository = orderRepository;
        _logger = logger;
    }

    [BindProperty]
    public CreateOrderModel CreateOrder { get; set; } = null!;

    public User UserData { get; set; } = null!;

    public float CartValue { get; set; }
    public float ShippingCost { get; set; } = 0.0f;

    public float OrderValue
        => CartValue + ShippingCost;

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var user = await _authService.GetLoggedInUserAsync(cancellationToken);
        UserData = user ?? throw new InvalidOperationException("User cannot be null");

        var cart = _cartService.GetCartEntries();
        CartValue = await _orderRepository.CalculateCartValueAsync(cart, cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Model: {@model}", CreateOrder);
        var cart = _cartService.GetCartEntries().ToList();
        if (!cart.Any())
        {
            throw new InvalidOperationException("Cannot place order with no items");
        }

        var user = await _authService.GetLoggedInUserAsync(cancellationToken);
        UserData = user ?? throw new InvalidOperationException("User cannot be null");

        var order = new Order()
        {
            UserId = UserData.Id,
            AddressLine = CreateOrder.AddressLine,
            City = CreateOrder.City,
            PostalCode = CreateOrder.Postal,
            Telephone = CreateOrder.Telephone
        };

        await _orderRepository.CreateOrderAsync(order, cart, cancellationToken);

        _cartService.ClearCart();

        return Redirect("/Index");
    }
}

public record CreateOrderModel(string AddressLine, string City, string Postal, string Telephone, string Comment);