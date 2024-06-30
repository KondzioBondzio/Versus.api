using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.ToTable("Rooms");

        builder.Property(x => x.Name)
            .HasMaxLength(64)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasMaxLength(8192)
            .IsRequired();

        builder.Property(x => x.Password)
            .HasMaxLength(32);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Rooms)
            .HasForeignKey(x => x.CategoryId)
            .IsRequired();

        builder.HasOne(x => x.Host)
            .WithMany(x => x.HostedRooms)
            .HasForeignKey(x => x.HostId)
            .IsRequired();

        builder.HasMany(x => x.Teams)
            .WithOne(x => x.Room)
            .HasForeignKey(x => x.RoomId)
            .IsRequired();

        builder.HasMany(x => x.Users)
            .WithOne(x => x.Room)
            .HasForeignKey(x => x.RoomId)
            .IsRequired();

        builder.HasMany(x => x.ChatMessages)
            .WithOne(x => x.Room)
            .HasForeignKey(x => x.RoomId)
            .IsRequired();
    }
}