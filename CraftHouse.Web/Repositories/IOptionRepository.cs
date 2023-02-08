using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface IOptionRepository
{
    Task<Product?> GetProductOptionsAsync(Product product, CancellationToken cancellationToken);
    Task DeleteOptionAsync(Option option, CancellationToken cancellationToken);
}