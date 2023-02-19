using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface IOrderItemRepository
{
    Task<List<OrderItem>> GetOrderItemByOrderIdAsync(int id, CancellationToken cancellationToken);
}