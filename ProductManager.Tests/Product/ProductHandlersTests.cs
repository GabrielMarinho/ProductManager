using System;
using System.Threading;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.Application.Endpoints.Product.Create;
using ProductManager.Application.Endpoints.Product.Delete;
using ProductManager.Application.Endpoints.Product.Get;
using ProductManager.Application.Endpoints.Product.Update;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Interfaces.Queue;
using ProductManager.Domain.Interfaces.Repository;
using Xunit;

namespace ProductManager.Tests.Product;

public class ProductHandlersTests
{
    [Fact]
    public async Task ProductCreate_Should_AddError_When_Name_Exists()
    {
        // Arrange
        var logger = new Mock<ILogger<ProductCreateHandler>>();
        var repo = new Mock<IProductRepository>();
        var queue = new Mock<IQueueEvents>();
        
        repo.Setup(r => r.GetByNameAsync("Existing"))
            .ReturnsAsync(new Domain.Entities.Product { Name = "Existing" });

        var handler = new ProductCreateHandler(logger.Object, repo.Object, queue.Object);

        // Act + Assert
        await Assert.ThrowsAsync<ValidationFailureException>(async () =>
        {
            await handler.HandleAsync(new ProductCreateRequest("Existing", 1, 10m), CancellationToken.None);
        });

        repo.Verify(r => r.InsertAsync(It.IsAny<Domain.Entities.Product>()), Times.Never);
        repo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ProductGet_Should_AddError_When_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<ProductGetHandler>>();
        var repo = new Mock<IProductRepository>();
        
        var id = Guid.NewGuid();
        repo.Setup(r => r.GetByIdentifierAsync(id))
            .ReturnsAsync((Domain.Entities.Product?)null);

        var handler = new ProductGetHandler(logger.Object, repo.Object);

        // Act + Assert
        await Assert.ThrowsAsync<ValidationFailureException>(async () =>
        {
            await handler.HandleAsync(new ProductGetRequest(id), CancellationToken.None);
        });
    }

    [Fact]
    public async Task ProductDelete_Should_AddError_When_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<ProductDeleteHandler>>();
        var repo = new Mock<IProductRepository>();
        
        var id = Guid.NewGuid();
        repo.Setup(r => r.GetByIdentifierAsync(id))
            .ReturnsAsync((Domain.Entities.Product?)null);

        var handler = new ProductDeleteHandler(logger.Object, repo.Object);

        // Act + Assert
        await Assert.ThrowsAsync<ValidationFailureException>(async () =>
        {
            await handler.HandleAsync(new ProductDeleteRequest(id), CancellationToken.None);
        });

        repo.Verify(r => r.DeleteByIdentifierAsync(It.IsAny<Guid>()), Times.Never);
        repo.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task ProductUpdate_Should_AddError_When_NotFound()
    {
        // Arrange
        var logger = new Mock<ILogger<ProductUpdateHandler>>();
        var repo = new Mock<IProductRepository>();
        
        var id = Guid.NewGuid();
        repo.Setup(r => r.GetByIdentifierAsync(id))
            .ReturnsAsync((Domain.Entities.Product?)null);

        var handler = new ProductUpdateHandler(logger.Object, repo.Object);

        // Act + Assert
        var request = new ProductUpdateRequest(id, "Name", 1, 10m);
        await Assert.ThrowsAsync<ValidationFailureException>(async () =>
        {
            await handler.HandleAsync(request, CancellationToken.None);
        });

        repo.Verify(r => r.UpdateAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<decimal>()), Times.Never);
        repo.Verify(r => r.CommitAsync(), Times.Never);
    }

}
