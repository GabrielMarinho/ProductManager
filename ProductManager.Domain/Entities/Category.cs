using BookRental.Domain.Identity;
using ProductManager.Domain.Interfaces.Entities;

namespace ProductManager.Domain.Entities;

public class Category : BaseEntity<int>, IIdentifiable
{
    public Guid Identifier { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
}