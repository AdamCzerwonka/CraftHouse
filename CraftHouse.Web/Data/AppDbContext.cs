using CraftHouse.Web.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Product>()
            .HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId);

        modelBuilder
            .Entity<Product>()
            .HasMany(x => x.Options)
            .WithMany(x => x.Products);

        modelBuilder
            .Entity<OptionValue>()
            .HasOne(x => x.Option)
            .WithMany(x => x.OptionValues)
            .HasForeignKey(x => x.OptionId);

        modelBuilder
            .Entity<OptionValue>()
            .HasKey(x => new { x.OptionId, x.Value });

        base.OnModelCreating(modelBuilder);
    }
}