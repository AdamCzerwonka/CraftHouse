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

    public async Task<List<Option>> GetOptionsByProductAsync(Product product, CancellationToken cancellationToken)
     => await _context
            .Options
            .AsNoTracking()
            .Where(x => x.ProductId == product.Id)
            .ToListAsync(cancellationToken);

    public Task<bool> IsOptionDeletedAsync(Option option, CancellationToken cancellationToken)
    {
        return Task.FromResult(option.DeletedAt != null);
    }

    public async Task DeleteOptionAsync(Option option, CancellationToken cancellationToken)
    {
        option.UpdatedAt = DateTime.Now;
        option.DeletedAt = DateTime.Now;
        _context.Options.Update(option);
        await _context.SaveChangesAsync(cancellationToken);
    }
}