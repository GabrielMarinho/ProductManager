using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Interfaces.Repository;
using ProductManager.Infrastructure.Context;

namespace ProductManager.Infrastructure.Repository;

public class CategoryRepository(ProductContext context) : 
    BaseRepository<Category>(context), ICategoryRepository
{
    public Task<List<Category>> GetAllAsync()
    {
        return context
            .Categories
            .AsNoTracking()
            .ToListAsync();
    }
}