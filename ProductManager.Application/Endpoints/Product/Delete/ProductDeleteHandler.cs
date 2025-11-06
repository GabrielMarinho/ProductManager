using FastEndpoints;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Interfaces.Repository;

namespace ProductManager.Application.Endpoints.Product.Delete;

public class ProductDeleteHandler(
    ILogger<ProductDeleteHandler> logger,
    IProductRepository productRepository) : 
    Endpoint<ProductDeleteRequest, ProductDeleteResponse>
{
    public override void Configure()
    {
        Delete("/product/{Identifier:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ProductDeleteRequest req, CancellationToken ct)
    {
        logger.LogInformation("Deleting product with identifier {Identifier}", req.Identifier);
        
        var product = await productRepository
            .GetByIdentifierAsync(req.Identifier);

        if (product is null)
        {
            AddError("Product not found.");
        }

        ThrowIfAnyErrors();
        
        await productRepository.DeleteByIdentifierAsync(req.Identifier);
        await productRepository.CommitAsync();

        await Send.OkAsync(new ProductDeleteResponse(), ct);
    }
}