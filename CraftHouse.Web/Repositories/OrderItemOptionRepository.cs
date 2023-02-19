using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Repositories;

public class OrderItemOptionRepository : IOrderItemOptionRepository
{
    private readonly AppDbContext _context;

    public OrderItemOptionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderItemOption>> GetOrderItemOptionByOrderItemIdAsync(int id,
        CancellationToken cancellationToken)
        => await _context.OrderItemOptions.Where(x => x.OrderItemId == id).AsNoTracking()
            .ToListAsync(cancellationToken);
}