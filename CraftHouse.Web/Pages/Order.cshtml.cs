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
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly ICartService _cartService;
    private readonly ILogger<OrderModel> _logger;
    private readonly IOrderRepository _orderRepository;

    public OrderModel(AppDbContext context, IAuthService authService, ICartService cartService,
        ILogger<OrderModel> logger, IOrderRepository orderRepository)
    {
        _context = context;
        _authService = authService;
        _cartService = cartService;
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var cart = _cartService.GetCartEntries();
        var user = await _authService.GetLoggedInUserAsync(cancellationToken);


        await _orderRepository.CreateOrderAsync(cart, user, cancellationToken);

        return Redirect("/Index");
    }
}