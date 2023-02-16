using CraftHouse.Web.Entities;
using CraftHouse.Web.Models;

namespace CraftHouse.Web.Repositories;

public interface IOrderRepository
{
   Task CreateOrderAsync(IEnumerable<CartEntry> cartEntries, User user, CancellationToken cancellationToken);
}