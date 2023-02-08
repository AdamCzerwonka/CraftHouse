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
        var options = await _optionRepository.GetOptionsByProduct(product);

        foreach (var option in options)
        {
            await _optionRepository.DeleteOptionAsync(option, cancellationToken);
        }
        
        product.UpdatedAt = DateTime.Now;
        product.DeletedAt = DateTime.Now;

        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }
}