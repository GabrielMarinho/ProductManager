using FastEndpoints;

namespace ProductManager.Application.Endpoints.Product.Get;

public class ProductGetHandler : Endpoint<ProductGetRequest, ProductGetResponse>
{
    public override void Configure()
    {
        Get("/product/{Identifier:guid}");
    }

    public override async Task HandleAsync(ProductGetRequest req, CancellationToken ct)
    {
        await Task.CompletedTask;
    }
}