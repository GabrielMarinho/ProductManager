using BookRental.Domain.Identity;
using ProductManager.Domain.Interfaces.Entities;

namespace ProductManager.Domain.Entities;

public class Product : BaseEntity<int>, IIdentifiable
{
    public Guid Identifier { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int IdCategory { get; set; }
    public decimal UnitCost { get; set; }
    
    public Category? Category { get; set; }
}