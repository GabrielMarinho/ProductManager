using ProductManager.Domain.Dtos;

namespace ProductManager.Application.Endpoints.Product.List;

public record ProductListResponse(int total, IEnumerable<ProductDto> products);