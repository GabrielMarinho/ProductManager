using ProductManager.Domain.Dtos;

namespace ProductManager.Application.Endpoints.Product.Create;

public record ProductCreateResponse(ProductDto Product);