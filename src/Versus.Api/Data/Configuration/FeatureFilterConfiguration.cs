using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class FeatureFilterConfiguration : IEntityTypeConfiguration<FeatureFilter>
{
    public void Configure(EntityTypeBuilder<FeatureFilter> builder)
    {
        builder.ToTable("FeatureFilters");

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Feature)
            .WithMany(x => x.Filters)
            .HasForeignKey(x => x.FeatureId)
            .IsRequired();

        builder.HasMany(x => x.Parameters)
            .WithOne(x => x.FeatureFilter)
            .HasForeignKey(x => x.FeatureFilterId)
            .IsRequired();
    }
}