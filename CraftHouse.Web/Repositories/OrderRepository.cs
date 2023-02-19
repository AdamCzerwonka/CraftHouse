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

    public async Task CreateOrderAsync(Order order, IEnumerable<CartEntry> cartEntries,
        CancellationToken cancellationToken)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.Execute(async () => await CreateOrderAsyncTransaction(order, cartEntries, cancellationToken));
    }

    public async Task<float> CalculateCartValueAsync(IEnumerable<CartEntry> cartEntries,
        CancellationToken cancellationToken)
    {
        var value = 0.0f;
        foreach (var cartEntry in cartEntries)
        {
            var product = await _productRepository.GetProductByIdAsync(cartEntry.ProductId, cancellationToken);
            value += product!.Price;

            if (cartEntry.Options is null)
            {
                continue;
            }

            foreach (var cartEntryOption in cartEntry.Options)
            {
                var optionValues = await _context
                    .OptionValues
                    .AsNoTracking()
                    .Where(x => x.OptionId == cartEntryOption.OptionId)
                    .ToListAsync(cancellationToken);

                foreach (var optionValue in cartEntryOption.Values)
                {
                    value += optionValues.First(x => x.Id == optionValue).Price;
                }
            }
        }

        return value;
    }

    private async Task CreateOrderAsyncTransaction(Order order, IEnumerable<CartEntry> cartEntries,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
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

                        _context.OrderItemOptions.Add(orderItemOption);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
            }


            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError("{@error}", e.Message);
            await transaction.RollbackAsync(cancellationToken);
        }
    }
}