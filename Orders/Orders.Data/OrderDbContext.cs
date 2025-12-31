using Microsoft.EntityFrameworkCore;

namespace Orders.Data;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Models.Order> Orders { get; set; } = null!;
    public DbSet<Models.OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.ClrType.Name);
        }

        modelBuilder.Entity<Models.Order>()
            .HasMany(e => e.Items)
            .WithOne()
            .HasForeignKey(f => f.OrderId);
    }
}