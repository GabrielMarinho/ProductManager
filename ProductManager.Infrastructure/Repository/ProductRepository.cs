using Microsoft.EntityFrameworkCore;
using ProductManager.Domain.Entities;
using ProductManager.Domain.Interfaces.Repository;
using ProductManager.Infrastructure.Context;

namespace ProductManager.Infrastructure.Repository;

public class ProductRepository(ProductContext context) : 
    BaseRepository<Product>(context), IProductRepository
{
    public async Task<(int total, IEnumerable<Product>? products)> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = Context.Products.AsQueryable();

        var totalRecords = await query.CountAsync();
        var result = await query
            .OrderBy(e => e.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalRecords, result);
    }

    public Task<Product?> GetByNameAsync(string name)
    {
        throw new NotImplementedException();
    }
}