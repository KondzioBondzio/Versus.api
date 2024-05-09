using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class FeatureFilterParameterConfiguration : IEntityTypeConfiguration<FeatureFilterParameter>
{
    public void Configure(EntityTypeBuilder<FeatureFilterParameter> builder)
    {
        builder.ToTable("FeatureFilterParameters");

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Value)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.FeatureFilter)
            .WithMany(x => x.Parameters)
            .HasForeignKey(x => x.FeatureFilterId)
            .IsRequired();
    }
}