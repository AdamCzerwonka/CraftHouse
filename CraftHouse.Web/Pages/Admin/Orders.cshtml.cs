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
    private readonly IUserRepository _userRepository;
    private readonly ILogger<Orders> _logger;

    public Orders(IOrderRepository orderRepository, IUserRepository userRepository, ILogger<Orders> logger)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _logger = logger;
    }
    
    [BindProperty]
    public int? UserId { get; set; }

    public List<Order> OrdersList { get; set; }

    public List<Entities.User> Users { get; set; }

    public string? OrderBy { get; set; }
    public bool? IsAscending { get; set; } = false;

    public async Task<IActionResult> OnGetAsync(int? userId, string? orderBy, bool? isAscending,
        CancellationToken cancellationToken)
    {
        OrderBy = orderBy;
        IsAscending = isAscending;
        IsAscending ??= false;

        if (userId is not null)
        {
            UserId = userId;
            var user = await _userRepository.GetUserByIdAsync((int)UserId, cancellationToken);

            if (user is null)
            {
                throw new NullReferenceException("User does not exists");
            }

            OrdersList =
                await _orderRepository.GetOrdersByUserIdWithSortingAsync((int)UserId, OrderBy, IsAscending,
                    cancellationToken);
        }
        else
        {
            OrdersList = await _orderRepository.GetOrdersWithSortingAsync(OrderBy, IsAscending, cancellationToken);
        }

        Users = await _userRepository.GetAllUsersAsync(cancellationToken);

        return Page();
    }
}