using FastEndpoints;

namespace ProductManager.Application.Endpoints.Product.Update;

public class ProducUpdateHandler : Endpoint<ProductUpdateRequest, ProductUpdateResponse>
{
    public override void Configure()
    {
        Put("/product/{Identifier:guid}");
    }

    public override async Task HandleAsync(ProductUpdateRequest req, CancellationToken ct)
    {
        await Task.CompletedTask;
    }
}