using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.ToTable("AuditLogs");
        
        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(x => x.EntityId)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(x => x.ChangeType)
            .HasConversion<byte>()
            .IsRequired();
        
        builder.Property(x => x.ChangedValues)
            .IsRequired();
        
        builder.Property(x => x.ChangedBy)
            .HasMaxLength(255);
        
        builder.Property(x => x.ChangeDate)
            .IsRequired();
    }
}