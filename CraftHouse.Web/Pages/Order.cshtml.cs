using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages;

[RequireAuth]
public class OrderModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ICartService _cartService;
    private readonly IOrderRepository _orderRepository;

    public OrderModel(IAuthService authService, ICartService cartService,
        IOrderRepository orderRepository)
    {
        _authService = authService;
        _cartService = cartService;
        _orderRepository = orderRepository;
    }

    [BindProperty]
    public CreateOrderModel CreateOrder { get; set; } = null!;

    public User UserData { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        var user =await _authService.GetLoggedInUserAsync(cancellationToken);
        UserData = user ?? throw new InvalidOperationException("User cannot be null");
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var cart = _cartService.GetCartEntries().ToList();
        if (!cart.Any())
        {
            throw new InvalidOperationException("Cannot place order with no items");
        }
        var user = await _authService.GetLoggedInUserAsync(cancellationToken);
        UserData = user ?? throw new InvalidOperationException("User cannot be null");

        await _orderRepository.CreateOrderAsync(cart, UserData, cancellationToken);
        
        _cartService.ClearCart();

        return Redirect("/Index");
    }
}

public record CreateOrderModel(string AddressLine, string City, string Postal, string Telephone);