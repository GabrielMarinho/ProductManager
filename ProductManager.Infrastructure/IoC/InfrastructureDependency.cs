using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductManager.Domain.Interfaces.Repository;
using ProductManager.Infrastructure.Context;
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
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                //options.UseNpgsql(connectionString);
            }
            else
            {
                throw new InvalidOperationException("Database can't be configured.");
            }
        });

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductEventsRepository, ProductEventsRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
    }
}