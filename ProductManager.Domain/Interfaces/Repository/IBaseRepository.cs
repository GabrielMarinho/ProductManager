namespace ProductManager.Domain.Interfaces.Repository;

public interface IBaseRepository<in TEntity> where TEntity : class
{
    Task InsertAsync(TEntity entity);
    
    Task CommitAsync();
}