using CraftHouse.Web.Entities;

namespace CraftHouse.Web.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<Category?> GetCategoryWithProducts(int id, CancellationToken cancellationToken);
    Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
    Task<bool> IsCategoryEmptyAsync(int id, CancellationToken cancellationToken);
    Task UpdateCategoryAsync(Category category, CancellationToken cancellationToken);
    Task<bool> CategoryExistsAsync(string name,CancellationToken cancellationToken);
    Task DeleteCategoryAsync(Category category, CancellationToken cancellationToken);
}