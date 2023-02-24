using CraftHouse.Web.Entities;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages;

public class OrderDetails : PageModel
{
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderItemOptionRepository _orderItemOptionRepository;

    public int OrderId { get; set; }
    public List<OrderItem> OrderItems { get; set; } = null!;
    public List<OrderItemOption> OrderItemOptions { get; set; } = null!;

    public OrderDetails(IOrderItemRepository orderItemRepository, IOrderItemOptionRepository orderItemOptionRepository)
    {
        _orderItemRepository = orderItemRepository;
        _orderItemOptionRepository = orderItemOptionRepository;
    }

    public async Task<IActionResult> OnGet(int orderId, CancellationToken cancellationToken)
    {
        OrderId = orderId;
        OrderItems = await _orderItemRepository.GetOrderItemByOrderIdAsync(orderId, cancellationToken);
        OrderItemOptions = new List<OrderItemOption>();
        foreach (var orderItem in OrderItems)
        {
            OrderItemOptions.AddRange(
                await _orderItemOptionRepository.GetOrderItemOptionByOrderItemIdAsync(orderItem.Id, cancellationToken));
        }

        return Page();
    }
}