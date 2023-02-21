using CraftHouse.Web.Entities;
using CraftHouse.Web.Models;

namespace CraftHouse.Web.Repositories;

public interface IOrderRepository
{
   Task CreateOrderAsync(IEnumerable<CartEntry> cartEntries, User user, CancellationToken cancellationToken);

   Task<List<Order>> GetOrdersByUserIdAsync(int id, CancellationToken cancellationToken);
   Task<List<Order>> GetOrdersByUserIdWithSortingAsync(int id, string? sortBy, bool? isAscending, CancellationToken cancellationToken);
   Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken);
   Task<List<Order>> GetOrdersWithSortingAsync(string? sortBy, bool? isAscending, CancellationToken cancellationToken);

   Task<Order?> GetOrderByIdAsync(int id, CancellationToken cancellationToken);

   Task UpdateOrderAsync(Order order, CancellationToken cancellationToken);
}