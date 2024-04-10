using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.Property(x => x.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(x => x.Room)
            .WithMany(x => x.Teams)
            .HasForeignKey(x => x.RoomId)
            .IsRequired();

        builder.HasMany(x => x.Users)
            .WithOne(x => x.Team)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}