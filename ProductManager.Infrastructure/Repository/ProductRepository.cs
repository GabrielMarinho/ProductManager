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
        var query = Context
            .Products
            .AsQueryable()
            .AsNoTracking();

        var totalRecords = await query.CountAsync();
        var result = await query
            .Join(Context.Categories, p => p.IdCategory, c => c.Id, (product, category) => new { p = product, c = category })
            .OrderBy(e => e.p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (totalRecords, result.Select(s =>
        {
            s.p.Category = s.c;
            return s.p;
        }));
    }

    public Task<Product?> GetByNameAsync(string name)
    {
        return context
            .Products
            .FirstOrDefaultAsync(x => x.Name.Equals(name));
    }

    public Task<Product?> GetByIdentifierAsync(Guid identifier)
    {
        return context
            .Products
            .FirstOrDefaultAsync(x => x.Identifier == identifier);
    }

    public Task DeleteByIdentifierAsync(Guid identifier)
    {
        return Context
            .Products
            .Where(w => w.Identifier == identifier)
            .ExecuteDeleteAsync();
    }

    public Task UpdateAsync(Guid identifier, string name, int idCategory, decimal unitCost)
    {
        return Context.Products.Where(w => w.Identifier == identifier)
            .ExecuteUpdateAsync(setters => 
                setters.SetProperty(s => s.Name, name)
                    .SetProperty(s => s.IdCategory, idCategory)
                    .SetProperty(s => s.UnitCost, unitCost)
                    .SetProperty(s => s.UpdatedAt, DateTime.UtcNow));
    }
}