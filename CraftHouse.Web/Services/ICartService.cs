namespace CraftHouse.Web.Services;

public interface ICartService
{
    void AddCartEntry(CartEntry entry);
    IEnumerable<CartEntry> GetCartEntries();
}