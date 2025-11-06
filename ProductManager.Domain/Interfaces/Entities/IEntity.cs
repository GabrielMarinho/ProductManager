namespace ProductManager.Domain.Interfaces.Entities;

public interface IEntity<T> where T : IEquatable<T>
{
    T Id { get; set; }
}