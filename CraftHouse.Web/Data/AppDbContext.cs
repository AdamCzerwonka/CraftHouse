using CraftHouse.Web.Entities;
using Microsoft.EntityFrameworkCore;

namespace CraftHouse.Web.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<OptionValue> OptionValues => Set<OptionValue>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderItemOption> OrderItemOptions => Set<OrderItemOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<OrderItemOption>()
            .HasOne(x => x.OrderItem)
            .WithMany(x => x.OrderItemOptions)
            .HasForeignKey(x => x.OrderItemId)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder
            .Entity<OrderItemOption>()
            .HasOne(x => x.Option)
            .WithMany(x => x.OrderItemOptions)
            .HasForeignKey(x => x.OptionId);

        modelBuilder
            .Entity<OrderItem>()
            .HasOne(x => x.Order)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.OrderId);

        modelBuilder
            .Entity<OrderItem>()
            .HasOne(x => x.Product)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.ProductId);

        modelBuilder
            .Entity<Order>()
            .HasOne(x => x.User)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.UserId);

        modelBuilder
            .Entity<Product>()
            .HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId);

        modelBuilder
            .Entity<Product>()
            .HasMany(x => x.Options)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId);

        modelBuilder
            .Entity<OptionValue>()
            .HasOne(x => x.Option)
            .WithMany(x => x.OptionValues)
            .HasForeignKey(x => x.OptionId);

        base.OnModelCreating(modelBuilder);
    }
}