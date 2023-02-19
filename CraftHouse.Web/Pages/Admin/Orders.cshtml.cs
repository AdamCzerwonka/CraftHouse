using CraftHouse.Web.Entities;
using CraftHouse.Web.Infrastructure;
using CraftHouse.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CraftHouse.Web.Pages.Admin;

[RequireAuth(UserType.Administrator)]
public class Orders : PageModel
{
    private readonly IOrderRepository _orderRepository;

    public Orders(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public  List<Order> OrdersList { get; set; }
    
    public async Task<IActionResult> OnGet(CancellationToken cancellationToken)
    {
        OrdersList = (await _orderRepository.GetOrdersAsync(cancellationToken));

        return Page();
    }
}