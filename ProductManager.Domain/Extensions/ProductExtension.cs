using ProductManager.Domain.Dtos;
using ProductManager.Domain.Entities;

namespace ProductManager.Domain.Extensions;

public static class ProductExtension
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto(
            product.Identifier,
            product.Name,
            product.IdCategory,
            product.UnitCost,
            product.Category?.ToDto());
    }
}