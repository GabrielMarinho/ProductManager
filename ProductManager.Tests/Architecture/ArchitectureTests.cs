using NetArchTest.Rules;
using ProductManager.Domain.Entities;
// use a domain type to get the assembly

namespace ProductManager.Tests.Architecture;

public class ArchitectureTests
{
    [Fact]
    public void Domain_Should_Not_Depend_On_Other_Layers()
    {
        // Arrange
        var domainAssembly = typeof(ProductManager.Domain.Entities.Product).Assembly;

        // Act
        var result = Types.InAssembly(domainAssembly)
            .Should()
            .NotHaveDependencyOn("ProductManager.Application")
            .And()
            .NotHaveDependencyOn("ProductManager.Infrastructure")
            .And()
            .NotHaveDependencyOn("ProductManager.Api")
            .GetResult();

        // Assert
        if (!result.IsSuccessful)
        {
            var failing = result.FailingTypes?.Select(t => t.FullName) ?? Enumerable.Empty<string?>();
            var message = "Domain layer has forbidden dependencies: " + string.Join(", ", failing);
            Assert.Fail(message);
        }
        else
        {
            Assert.True(true);
        }
    }
}
