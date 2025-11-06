namespace ProductManager.Domain.Interfaces.Entities;

public interface IIdentifiable
{
    Guid Identifier { get; set; }
}