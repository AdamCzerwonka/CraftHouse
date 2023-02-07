using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
    => await _context
        .Categories
        .Where(x => x.DeletedAt == null)
        .ToListAsync(cancellationToken);

    public async Task<Category?> GetCategoryWithProducts(int id, CancellationToken cancellationToken)
        => await _context
            .Categories
            .Include(x => x.Products)
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

    public async Task<Category?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
        => await _context
            .Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<bool> IsCategoryEmptyAsync(int id, CancellationToken cancellationToken)
        => await _context
            .Products
            .Where(x => x.DeletedAt == null)
            .AnyAsync(x => x.CategoryId == id, cancellationToken);

    public async Task UpdateCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        category.UpdatedAt = DateTime.Now;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> CategoryExistsAsync(string name, CancellationToken cancellationToken)
        => await _context.Categories.AnyAsync(x => x.Name == name, cancellationToken);

    public async Task DeleteCategoryAsync(Category category, CancellationToken cancellationToken)
    {
        category.DeletedAt = DateTime.Now;
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
    }
}