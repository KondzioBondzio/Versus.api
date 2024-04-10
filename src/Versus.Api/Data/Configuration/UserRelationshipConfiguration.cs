using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class UserRelationshipConfiguration : IEntityTypeConfiguration<UserRelationship>
{
    public void Configure(EntityTypeBuilder<UserRelationship> builder)
    {
        builder.ToTable("UserRelationships");

        builder.HasOne(x => x.User)
            .WithMany(x => x.RelatedUsers)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasOne(x => x.RelatedUser)
            .WithMany(x => x.RelatedTo)
            .HasForeignKey(x => x.RelatedUserId)
            .IsRequired();
    }
}