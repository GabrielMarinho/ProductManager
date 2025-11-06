using FastEndpoints;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Dtos;
using ProductManager.Domain.Extensions;
using ProductManager.Domain.Interfaces.Repository;

namespace ProductManager.Application.Endpoints.Product.List;

public class ProductListHandler(
    ILogger<ProductListHandler> logger,
    IProductRepository productRepository) : 
    Endpoint<ProductListRequest, ProductListResponse>
{
    public override void Configure()
    {
        Get("/products");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(ProductListRequest req, CancellationToken ct)
    {
        logger.LogInformation("Handling Product List Request. Page Number: {PageNumber}, Page Size: {PageSize}", 
            req.PageNumber,
            req.PageSize);
        
        var response = await productRepository
            .GetAllAsync(req.PageNumber, req.PageSize);
    
        await Send.OkAsync(new ProductListResponse(
            response.total,
            response.products?
                .Select(s => s.ToDto())
                .ToList() ?? Enumerable.Empty<ProductDto>()
        ), ct);
    }
}