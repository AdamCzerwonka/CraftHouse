using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken);

    Task<List<Product>> GetProductsByCategoryWithSortingAsync(int pageNumber, int? categoryId, string? sortBy,
        bool? isAscending, CancellationToken cancellationToken = default);
    Task<int> GetProductsCountByCategoryAsync(int? categoryId, CancellationToken cancellationToken);
    Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
    Task AddProductAsync(Product product, CancellationToken cancellationToken);
    Task DeleteProductWithOptionsAsync(Product product, CancellationToken cancellationToken);
    Task UpdateProductAsync(Product product, CancellationToken cancellationToken);
}