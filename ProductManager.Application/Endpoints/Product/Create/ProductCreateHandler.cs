using FastEndpoints;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Extensions;
using ProductManager.Domain.Interfaces.Queue;
using ProductManager.Domain.Interfaces.Repository;

namespace ProductManager.Application.Endpoints.Product.Create;

public class ProductCreateHandler(
    ILogger<ProductCreateHandler> logger,
    IProductRepository productRepository,
    IQueueEvents queueEvents) :
    Endpoint<ProductCreateRequest, ProductCreateResponse>
{
    public override void Configure()
    {
        Post("/product");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(ProductCreateRequest req, CancellationToken ct)
    {
        logger.LogInformation("Creating new product with name {Name}", req.Name);
        
        var product = await productRepository.GetByNameAsync(req.Name);
        if (product is not null)
        {
            AddError("Already exists a product with this name. Please, consider change the name.", req.Name);
        }
        
        ThrowIfAnyErrors();

        product = new Domain.Entities.Product()
        {
            Name = req.Name,
            IdCategory = req.IdCategory,
            UnitCost = req.UnitCost
        };

        await productRepository.InsertAsync(product);
        await productRepository.CommitAsync();

        var productDto = product.ToDto();
        await queueEvents.PublishAsync("product.created", productDto, ct);

        await Send.OkAsync(new ProductCreateResponse(productDto), ct);
    }
}