using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class RoomUserConfiguration : IEntityTypeConfiguration<RoomUser>
{
    public void Configure(EntityTypeBuilder<RoomUser> builder)
    {
        builder.ToTable("RoomUsers");

        builder.HasOne(x => x.Room)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.RoomId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.RoomsInUse)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}