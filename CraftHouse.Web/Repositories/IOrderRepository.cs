using CraftHouse.Web.Entities;
using CraftHouse.Web.Models;

namespace CraftHouse.Web.Repositories;

public interface IOrderRepository
{
   Task CreateOrderAsync(Order order, IEnumerable<CartEntry> cartEntries,CancellationToken cancellationToken);
   Task<float> CalculateCartValueAsync(IEnumerable<CartEntry> cartEntries, CancellationToken cancellationToken);
}