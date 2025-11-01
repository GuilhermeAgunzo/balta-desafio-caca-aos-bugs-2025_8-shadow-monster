using BugStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BugStore.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.Title)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired(false);

        builder.Property(x => x.Price)
            .IsRequired();

        builder.Property(x => x.Slug)
            .IsRequired();

        builder.Ignore(x => x.Notifications);
    }
}
