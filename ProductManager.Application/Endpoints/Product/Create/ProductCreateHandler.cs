using FastEndpoints;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Interfaces.Repository;

namespace ProductManager.Application.Endpoints.Product.Create;

public class ProductCreateHandler(
    ILogger<ProductCreateHandler> logger,
    IProductRepository productRepository) :
    Endpoint<ProductCreateRequest, ProductCreateResponse>
{
    public override void Configure()
    {
        Post("/product");
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

        
        await Task.CompletedTask;
    }
}