using ProductManager.Domain.Entities;

namespace ProductManager.Domain.Interfaces.Repository;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<List<Category>> GetAllAsync();
}