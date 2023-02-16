using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Repositories;

public class OptionRepository : IOptionRepository
{
    private readonly AppDbContext _context;

    public OptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetProductOptionsAsync(Product product, CancellationToken cancellationToken)
        => await _context
            .Products
            .Include(x => x.Options)
            .FirstOrDefaultAsync(x => x.Id == product.Id, cancellationToken);

    public async Task<Option?> GetOptionByIdAsync(int id, CancellationToken cancellationToken)
        => await _context
            .Options
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, cancellationToken);

    public async Task AddOptionAsync(Option option, CancellationToken cancellationToken)
    {
        option.CreatedAt = DateTime.Now;
        option.UpdatedAt = DateTime.Now;
        _context.Options.Add(option);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Option>> GetOptionsByProductAsync(Product product, CancellationToken cancellationToken)
        => await _context
            .Options
            .AsNoTracking()
            .Where(x => x.ProductId == product.Id && x.DeletedAt == null)
            .ToListAsync(cancellationToken);

    public async Task<List<Option>> GetOptionsByProductIdAsync(int productId, CancellationToken cancellationToken)
        => await _context
            .Options
            .AsNoTracking()
            .Where(x => x.ProductId == productId && x.DeletedAt == null)
            .ToListAsync(cancellationToken);

    public async Task<bool> IsOptionDeletedAsync(Option option, CancellationToken cancellationToken)
        => await _context
            .Options
            .AnyAsync(x => x.Id == option.Id && x.DeletedAt != null, cancellationToken);

    public async Task UpdateOptionAsync(Option option, CancellationToken cancellationToken)
    {
        option.UpdatedAt = DateTime.Now;
        _context.Options.Update(option);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOptionAsync(Option option, CancellationToken cancellationToken)
    {
        option.UpdatedAt = DateTime.Now;
        option.DeletedAt = DateTime.Now;
        _context.Options.Update(option);
        await _context.SaveChangesAsync(cancellationToken);
    }
}