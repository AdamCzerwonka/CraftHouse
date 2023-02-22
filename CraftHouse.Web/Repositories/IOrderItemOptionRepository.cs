using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface IOrderItemOptionRepository
{
    Task<List<OrderItemOption>> GetOrderItemOptionByOrderItemIdAsync(int id, CancellationToken cancellationToken);
}