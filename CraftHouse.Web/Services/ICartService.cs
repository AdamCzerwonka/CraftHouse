using CraftHouse.Web.Models;

namespace CraftHouse.Web.Services;

public interface ICartService
{
    void AddCartEntry(CartEntry entry);
    IEnumerable<CartEntry> GetCartEntries();
    void RemoveCartEntry(Guid entryId);
    void ClearCart();
}