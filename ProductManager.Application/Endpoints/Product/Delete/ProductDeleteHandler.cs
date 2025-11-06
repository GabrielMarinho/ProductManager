using FastEndpoints;

namespace ProductManager.Application.Endpoints.Product.Delete;

public class ProductDeleteHandler : Endpoint<ProductDeleteRequest, ProductDeleteResponse>
{
    public override void Configure()
    {
        Delete("/product/{Identifier:guid}");
    }

    public override async Task HandleAsync(ProductDeleteRequest req, CancellationToken ct)
    {
        
        
        
        await Task.CompletedTask;
    }
}