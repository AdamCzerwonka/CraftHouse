using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface IOptionRepository
{
    Task<Product?> GetProductOptionsAsync(Product product, CancellationToken cancellationToken);
    Task<Option?> GetOptionByIdAsync(int id, CancellationToken cancellationToken);
    Task AddOptionAsync(Option option, CancellationToken cancellationToken);
    Task DeleteOptionAsync(Option option, CancellationToken cancellationToken);
    Task<List<Option>> GetOptionsByProductAsync(Product product, CancellationToken cancellationToken);
    Task<List<Option>> GetOptionsByProductIdAsync(int productId, CancellationToken cancellationToken);
    Task<bool> IsOptionDeletedAsync(Option option, CancellationToken cancellationToken);
    Task UpdateOptionAsync(Option option, CancellationToken cancellationToken);
    Task<List<Option>> GetOptionsWithOptionValuesByProductIdAsync(int productId, CancellationToken cancellationToken);
}