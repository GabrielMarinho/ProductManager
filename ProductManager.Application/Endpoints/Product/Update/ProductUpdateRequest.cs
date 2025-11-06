namespace ProductManager.Application.Endpoints.Product.Update;

public record ProductUpdateRequest(Guid Identifier, string Name, int IdCategory, decimal UnitCost);