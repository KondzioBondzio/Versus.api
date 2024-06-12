using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(250);
        
        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.HasMany(x => x.Rooms)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .IsRequired();
    }
}