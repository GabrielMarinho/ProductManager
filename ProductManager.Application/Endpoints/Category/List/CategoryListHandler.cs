using FastEndpoints;
using Microsoft.Extensions.Logging;
using ProductManager.Domain.Extensions;
using ProductManager.Domain.Interfaces.Repository;

namespace ProductManager.Application.Endpoints.Category.List;

public class CategoryListHandler(
    ILogger<CategoryListHandler> logger,
    ICategoryRepository categoryRepository) : EndpointWithoutRequest<CategoryListResponse>
{
    public override void Configure()
    {
        Get("/categories");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        logger.LogInformation("Handling Category List Request.");

        var categories = await categoryRepository
            .GetAllAsync();

        await Send.OkAsync(new CategoryListResponse(
            categories
                .Select(s => s.ToDto())), ct);
    }
}