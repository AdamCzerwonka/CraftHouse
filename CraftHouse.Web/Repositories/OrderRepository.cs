using CraftHouse.Web.Data;
using CraftHouse.Web.Entities;
using CraftHouse.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<OrderRepository> _logger;

    public OrderRepository(AppDbContext context, IProductRepository productRepository, ILogger<OrderRepository> logger)
    {
        _context = context;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task CreateOrderAsync(IEnumerable<CartEntry> cartEntries, User user,
        CancellationToken cancellationToken)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.Execute(async () => await CreateOrderAsyncTransaction(cartEntries, user, cancellationToken));
    }

    public async Task<List<Order>> GetOrdersByUserAsync(int id, CancellationToken cancellationToken)
        => await _context.Orders.Where(x => x.UserId == id).AsNoTracking().ToListAsync(cancellationToken);

    public async Task<List<Order>> GetOrdersAsync(CancellationToken cancellationToken)
        => await _context.Orders.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Order?> GetOrderByIdAsync(int id, CancellationToken cancellationToken)
        => await _context.Orders.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public async Task UpdateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        order.UpdatedAt = DateTime.Now;
        _context.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task CreateOrderAsyncTransaction(IEnumerable<CartEntry> cartEntries, User user,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        float totalCost = 0;
        
        try
        {
            var order = new Order()
            {
                AddressLine = user.AddressLine,
                City = user.City,
                PostalCode = user.PostalCode,
                Telephone = user.TelephoneNumber,
                UserId = user.Id
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            foreach (var entry in cartEntries)
            {
                var product = await _productRepository.GetProductByIdAsync(entry.ProductId, cancellationToken);

                if (product is null)
                {
                    throw new Exception("Product not found");
                }

                var orderItem = new OrderItem()
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Value = product.Price
                };

                totalCost += product.Price;

                _context.OrderItems.Add(orderItem);
                await _context.SaveChangesAsync(cancellationToken);

                if (entry.Options is null)
                {
                    continue;
                }

                foreach (var entryOption in entry.Options)
                {
                    var option = await _context
                        .Options
                        .Include(x => x.OptionValues)
                        .AsNoTracking()
                        .Where(x => x.DeletedAt == null)
                        .FirstAsync(x => x.Id == entryOption.OptionId, cancellationToken);

                    foreach (var optionValue in entryOption.Values)
                    {
                        var value = option.OptionValues.First(x => x.Id == optionValue);
                        var orderItemOption = new OrderItemOption()
                        {
                            OrderItemId = orderItem.Id,
                            OptionId = option.Id,
                            Name = value.Value,
                            Value = value.Price
                        };

                        totalCost += value.Price;

                        _context.OrderItemOptions.Add(orderItemOption);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
            }

            order.Value = totalCost;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("{@error}", e.Message);
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}