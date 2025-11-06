namespace ProductManager.Domain.Dtos;

public record ProductDto(
    Guid Identifier,
    string Name, 
    int IdCategory,
    decimal UnitCost,
    CategoryDto? Category = null);