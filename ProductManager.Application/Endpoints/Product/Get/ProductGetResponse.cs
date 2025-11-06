using ProductManager.Domain.Dtos;

namespace ProductManager.Application.Endpoints.Product.Get;

public record ProductGetResponse(ProductDto Product);