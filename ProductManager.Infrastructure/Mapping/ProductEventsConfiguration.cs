using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Domain.Entities;

namespace ProductManager.Infrastructure.Mapping;

public class ProductEventsConfiguration : IEntityTypeConfiguration<ProductEvents>
{
    public void Configure(EntityTypeBuilder<ProductEvents> builder)
    {
        builder.ToTable("ProductEvents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Identifier)
            .IsRequired()
            .HasDefaultValueSql("NEWID()");

        builder.HasIndex(x => x.Identifier).IsUnique();

        builder.Property(x => x.Event)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.Active)
            .HasDefaultValue(true);
    }
}