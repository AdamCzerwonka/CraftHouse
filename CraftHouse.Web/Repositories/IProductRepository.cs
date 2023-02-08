using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken);
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
    Task AddProductAsync(Product product, CancellationToken cancellationToken);
    Task DeleteProductWithOptionsAsync(Product product, CancellationToken cancellationToken);
}