using System.Reflection;
using BookRental.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Entities;

namespace ProductManager.Infrastructure.Context;

public class ProductContext(DbContextOptions<ProductContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductEvents> ProductEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        modelBuilder
            .Entity<BaseEntity<int>>()
            .HasQueryFilter(q => q.Active)
            .ToTable(nameof(BaseEntity<int>), t => t.ExcludeFromMigrations());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseAsyncSeeding(async (context, _, cancellationToken) =>
        {
            var total = await context.Set<Category>().CountAsync(cancellationToken);
            if (total > 0)
                return;

            context.Set<Category>().Add(new Category()
            {
                Id = 1, 
                Name = "Drink",
                Identifier = Guid.NewGuid(), 
                CreatedAt = DateTime.Now,
                Active = true
            });
            
            await context.SaveChangesAsync(cancellationToken);
        });
        
        base.OnConfiguring(optionsBuilder);
    }
}