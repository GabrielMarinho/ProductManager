namespace ProductManager.Application.Endpoints.Product.Create;

public record ProductCreateRequest(string Name, int IdCategory, decimal UnitCost);