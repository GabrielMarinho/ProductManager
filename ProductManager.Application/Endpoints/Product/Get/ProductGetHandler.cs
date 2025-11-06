using FastEndpoints;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Extensions;
using ProductManager.Domain.Interfaces.Repository;

namespace ProductManager.Application.Endpoints.Product.Get;

public class ProductGetHandler(
    ILogger<ProductGetHandler> logger,
    IProductRepository productRepository) :
    Endpoint<ProductGetRequest, ProductGetResponse>
{
    public override void Configure()
    {
        Get("/product/{Identifier:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ProductGetRequest req, CancellationToken ct)
    {
        logger.LogInformation("Handling Product Get Request. Identifier: {Identifier}", req.Identifier);
        
        var product = await productRepository.GetByIdentifierAsync(req.Identifier);
        if (product is null)
        {
            AddError("Product not found.");
        }
        
        ThrowIfAnyErrors();

        await Send.OkAsync(new ProductGetResponse(product.ToDto()), ct);
    }
}