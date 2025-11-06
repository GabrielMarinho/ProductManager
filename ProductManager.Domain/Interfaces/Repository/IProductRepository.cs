using ProductManager.Domain.Entities;

namespace ProductManager.Domain.Interfaces.Repository;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<(int total, IEnumerable<Product>? products)> GetAllAsync(int pageNumber, int pageSize);
    Task<Product?> GetByNameAsync(string name);
    Task<Product?> GetByIdentifierAsync(Guid identifier);
    Task DeleteByIdentifierAsync(Guid identifier);
    Task UpdateAsync(Guid identifier, string name, int idCategory, decimal unitCost);
}