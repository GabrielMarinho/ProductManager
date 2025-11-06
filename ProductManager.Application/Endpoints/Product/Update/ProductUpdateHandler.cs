using FastEndpoints;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Extensions;
using ProductManager.Domain.Interfaces.Repository;

namespace ProductManager.Application.Endpoints.Product.Update;

public class ProductUpdateHandler(
    ILogger<ProductUpdateHandler> logger,
    IProductRepository productRepository) : Endpoint<ProductUpdateRequest, ProductUpdateResponse>
{
    public override void Configure()
    {
        Put("/product/{Identifier:guid}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ProductUpdateRequest req, CancellationToken ct)
    {
        logger.LogInformation("Updating product with identifier {Identifier}", req.Identifier);
        
        var product = await productRepository.GetByIdentifierAsync(req.Identifier);
        if (product is null)
        {
            AddError("Product not found.");
        }
        
        ThrowIfAnyErrors();
        
        await productRepository.UpdateAsync(req.Identifier, req.Name, req.IdCategory, req.UnitCost);

        await Send.OkAsync(new ProductUpdateResponse(product.ToDto()), ct);
    }
}