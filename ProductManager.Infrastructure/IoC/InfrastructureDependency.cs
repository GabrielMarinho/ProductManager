using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Domain.Interfaces.Queue;
using ProductManager.Domain.Interfaces.Repository;
using ProductManager.Infrastructure.Context;
using ProductManager.Infrastructure.Queue;
using ProductManager.Infrastructure.Repository;

namespace ProductManager.Infrastructure.IoC;

public static class InfrastructureDependency
{
    public static void RegisterInfrastructureDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ProductContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION") ?? configuration.GetConnectionString("DefaultConnection");

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                throw new InvalidOperationException("Database can't be configured.");
            }
        });

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductEventsRepository, ProductEventsRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();

        // Queue events
        services.AddSingleton<IQueueEvents, RabbitMqQueueEvents>();
    }
    
    public static async Task ExecuteDataBaseMigrationAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();
        await dbContext.Database.MigrateAsync();
    }
}