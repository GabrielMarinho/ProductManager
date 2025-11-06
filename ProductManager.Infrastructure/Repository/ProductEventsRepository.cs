using ProductManager.Domain.Entities;
using ProductManager.Domain.Interfaces.Repository;
using ProductManager.Infrastructure.Context;

namespace ProductManager.Infrastructure.Repository;

public class ProductEventsRepository(ProductContext context) : 
    BaseRepository<ProductEvents>(context), IProductEventsRepository
{
    
}