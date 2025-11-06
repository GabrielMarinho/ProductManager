using Newtonsoft.Json;
using ProductManager.Domain.Dtos;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Interfaces.Queue;
using ProductManager.Domain.Interfaces.Repository;
using ProductManager.Infrastructure.Queue;

namespace ProductManager.Api.BackgroundService;

public class ProductCreatedConsumer(
    IServiceProvider serviceProvider) : Microsoft.Extensions.Hosting.BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ProductCreatedConsumer>>();
        var queue = scope.ServiceProvider.GetRequiredService<IQueueEvents>();
        
        queue.Subscribe<ProductDto>(RabbitMqQueueEvents.ProductCreatedQueue, async message =>
        {
            logger.LogInformation("[Consumer] Product created event consumed: {Name} ({Id})", message.Name, message.Identifier);
            
            var productEventsRepository = scope.ServiceProvider.GetRequiredService<IProductEventsRepository>();
            
            await productEventsRepository.InsertAsync(new ProductEvents()
            {
                Event = JsonConvert.SerializeObject(message)
            });
            await productEventsRepository.CommitAsync();
            
            await Task.CompletedTask;
        });

        return Task.CompletedTask;
    }
}
