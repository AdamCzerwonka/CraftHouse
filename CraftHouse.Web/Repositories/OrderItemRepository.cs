using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly AppDbContext _context;

    public OrderItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderItem>> GetOrderItemByOrderIdAsync(int id, CancellationToken cancellationToken)
        => await _context.OrderItems.Include(x => x.Product.Category).Where(x => x.OrderId == id).AsNoTracking().ToListAsync(cancellationToken);
}