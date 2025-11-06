using ProductManager.Domain.Interfaces.Repository;
using ProductManager.Infrastructure.Context;

namespace ProductManager.Infrastructure.Repository;

public class BaseRepository<TEntity>(ProductContext context) : IBaseRepository<TEntity> where TEntity : class
{
    protected ProductContext Context => context;
    
}