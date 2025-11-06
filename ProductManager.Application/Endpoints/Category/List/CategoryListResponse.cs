using ProductManager.Domain.Dtos;

namespace ProductManager.Application.Endpoints.Category.List;

public record CategoryListResponse(IEnumerable<CategoryDto> Categories);