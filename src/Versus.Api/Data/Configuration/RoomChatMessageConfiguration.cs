using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class RoomChatMessageConfiguration : IEntityTypeConfiguration<RoomChatMessage>
{
    public void Configure(EntityTypeBuilder<RoomChatMessage> builder)
    {
        builder.ToTable("RoomChatMessages");

        builder.HasOne(x => x.Room)
            .WithMany(x => x.ChatMessages)
            .HasForeignKey(x => x.RoomId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.ChatMessages)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasMaxLength(250)
            .IsRequired();
    }
}