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

    public async Task<List<Option>> GetOptionsByProduct(Product product)
    {
        var options = await _context
            .Options
            .AsNoTracking()
            .Where(x => x.Products.Any(product1 => product1.Id == product.Id))
            .ToListAsync();

        return options;
    }

    public async Task DeleteOptionAsync(Option option, CancellationToken cancellationToken)
    {
        option.UpdatedAt = DateTime.Now;
        option.DeletedAt = DateTime.Now;
        _context.Options.Update(option);
        await _context.SaveChangesAsync(cancellationToken);
    }
}