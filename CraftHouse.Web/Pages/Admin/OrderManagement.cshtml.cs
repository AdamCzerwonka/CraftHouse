using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class OrderManagement : PageModel
{
    private readonly IOrderRepository _orderRepository;

    public OrderManagement(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    [BindProperty]
    public int OrderId { get; set; }

    [BindProperty]
    public Order OrderData { get; set; }
    
    [BindProperty]
    public OrderStatus OrderStatus { get; set; }
    
    
    public async Task<IActionResult> OnGetAsync(int orderId, CancellationToken cancellationToken)
    {
        OrderId = orderId;

        var order = await _orderRepository.GetOrderByIdAsync(orderId, cancellationToken);
        OrderData = order ?? throw new NullReferenceException("Order does not exists");
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(OrderId, cancellationToken);

        if (order is null)
        {
            throw new NullReferenceException("Order does not exists");
        }

        order.OrderStatus = OrderStatus;
        await _orderRepository.UpdateOrderAsync(order, cancellationToken);
        
        return Redirect($"/Admin/OrderManagement/{OrderId}");
    }
}