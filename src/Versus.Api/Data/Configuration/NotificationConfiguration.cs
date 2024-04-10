using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        
        builder.Property(x => x.Title)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasMaxLength(250)
            .IsRequired();

        builder.HasMany(x => x.NotificationUsers)
            .WithOne(x => x.Notification)
            .HasForeignKey(x => x.NotificationId)
            .IsRequired();
    }
}