namespace CraftHouse.Web.Models;

public class CartEntry
{
    public int ProductId { get; init; }
    public IEnumerable<CartEntryOption>? Options { get; init; }
}

public class CartEntryOption
{
    public int OptionId { get; init; }
    public List<int> Values { get; init; } = null!;
}
