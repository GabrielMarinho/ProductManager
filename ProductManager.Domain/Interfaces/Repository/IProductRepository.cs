using ProductManager.Domain.Entities;

namespace ProductManager.Domain.Interfaces.Repository;

public interface IProductRepository
{
    Task<(int total, IEnumerable<Product>? products)> GetAllAsync(int pageNumber, int pageSize);
    Task<Product?> GetByNameAsync(string name);
}