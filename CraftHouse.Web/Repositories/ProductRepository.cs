using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    private readonly IOptionRepository _optionRepository;

    public ProductRepository(AppDbContext context, IOptionRepository optionRepository)
    {
        _context = context;
        _optionRepository = optionRepository;
    }

    public async Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken)
        => await _context
            .Products
            .Include(x => x.Category)
            .Where(x => x.DeletedAt == null)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<List<Product>> GetProductsByCategoryWithSortingAsync(int pageNumber, int? categoryId, string? sortBy, bool? isAscending,
        CancellationToken cancellationToken = default)
    {
        var query = _context
            .Products
            .AsNoTracking()
            .Where(x => x.DeletedAt == null);

        if (categoryId is not null)
        {
            query = query.Where(x => x.CategoryId == categoryId);
        }
        
        sortBy = sortBy?.ToLower();
        isAscending ??= true;

        query = (sortBy, isAscending) switch
        {
            ("name", true) => query.OrderBy(x => x.Name),
            ("price", true) => query.OrderBy(x => x.Price),
            ("name", false) => query.OrderByDescending(x => x.Name),
            ("price", false) => query.OrderByDescending(x => x.Price),
            _ => query.OrderBy(x => x.Id)
        };

        const int productsPerPage = 15;
        var productsToSkip = productsPerPage * (pageNumber - 1);
        
        return await query
            .Skip(productsToSkip)
            .Take(productsPerPage)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetProductsCountByCategoryAsync(int? categoryId, CancellationToken cancellationToken)
    {
        var query = _context
            .Products
            .AsNoTracking()
            .Where(x => x.DeletedAt == null);

        if (categoryId is not null)
        {
            query = query.Where(x => x.CategoryId == categoryId);
        }

        return await query.CountAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
        => await _context
            .Products
            .Where(x => x.DeletedAt == null)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddProductAsync(Product product, CancellationToken cancellationToken)
    {
        product.CreatedAt = DateTime.Now;
        product.UpdatedAt = DateTime.Now;
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProductWithOptionsAsync(Product product, CancellationToken cancellationToken)
    {
        var options = await _optionRepository.GetOptionsByProductAsync(product, cancellationToken);

        foreach (var option in options)
        {
            var isOptionDeleted = await _optionRepository.IsOptionDeletedAsync(option, cancellationToken);
            if (!isOptionDeleted)
            {
                await _optionRepository.DeleteOptionAsync(option, cancellationToken);
            }
        }

        product.UpdatedAt = DateTime.Now;
        product.DeletedAt = DateTime.Now;

        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProductAsync(Product product, CancellationToken cancellationToken)
    {
        product.UpdatedAt = DateTime.Now;
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}