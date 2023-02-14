using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Pages;

public class OrderModel : PageModel
{
    private readonly AppDbContext _context;
    private readonly IAuthService _authService;
    private readonly ICartService _cartService;
    private readonly ILogger<OrderModel> _logger;

    public OrderModel(AppDbContext context, IAuthService authService, ICartService cartService,
        ILogger<OrderModel> logger)
    {
        _context = context;
        _authService = authService;
        _cartService = cartService;
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var cart = _cartService.GetCartEntries();
        var user = await _authService.GetLoggedInUserAsync(cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.Execute(async () =>
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var order = new Order()
                {
                    AddressLine = user.AddressLine,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    Telephone = user.TelephoneNumber,
                    UserId = user.Id
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var entry in cart)
                {
                    var orderItem = new OrderItem()
                    {
                        OrderId = order.Id,
                        ProductId = entry.ProductId
                    };

                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError("{@error}", e.Message);
                await transaction.RollbackAsync(cancellationToken);
            }
        });


        return Redirect("/Index");
    }
}