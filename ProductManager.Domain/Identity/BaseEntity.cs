using ProductManager.Domain.Interfaces.Entities;

namespace BookRental.Domain.Identity;

public abstract class BaseEntity<TKey> : IEntity<TKey> 
    where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool Active { get; set; } = true;
}