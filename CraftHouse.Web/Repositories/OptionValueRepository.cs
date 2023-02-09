using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Repositories;

public class OptionValueRepository : IOptionValueRepository
{
    private readonly AppDbContext _context;

    public OptionValueRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OptionValue?> GetOptionValueAsync(int optionId, string value, CancellationToken cancellationToken)
        => await _context
            .OptionValues
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.OptionId == optionId && x.Value == value, cancellationToken);

    public async Task<List<OptionValue>> GetOptionValuesByOptionAsync(Option option, CancellationToken cancellationToken)
        => await _context
            .OptionValues
            .Where(x => x.OptionId == option.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<List<OptionValue>> GetOptionValuesByOptionIdAsync(int optionId, CancellationToken cancellationToken)
        => await _context
            .OptionValues
            .Where(x => x.OptionId == optionId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task AddNewOptionValueAsync(OptionValue optionValue, CancellationToken cancellationToken)
    {
        await _context.OptionValues.AddAsync(optionValue, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveOptionValueAsync(int optionId, string value, CancellationToken cancellationToken)
    {
        var optionValue =
            await _context.OptionValues
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OptionId == optionId && x.Value == value,
                cancellationToken);

        _context.OptionValues.Remove(optionValue!);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateOptionValueFieldsAsync(int optionId, string oldValue, string newValue, float newPrice,
        CancellationToken cancellationToken)
    {
        var optionValueToDelete =
            await _context.OptionValues
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OptionId == optionId && x.Value == oldValue,
                cancellationToken);
        
        _context.OptionValues.Remove(optionValueToDelete!);
        
        var newOptionValue = new OptionValue()
        {
            Value = newValue,
            Price = newPrice,
            OptionId = optionId
        };
        
        await _context.OptionValues.AddAsync(newOptionValue, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}