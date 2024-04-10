using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class TeamUserConfiguration : IEntityTypeConfiguration<TeamUser>
{
    public void Configure(EntityTypeBuilder<TeamUser> builder)
    {
        builder.ToTable("TeamUsers");

        builder.HasOne(x => x.Team)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.TeamId)
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.TeamsMember)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}