﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(x => x.UserName)
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(x => x.Email)
            .HasMaxLength(254)
            .IsRequired();

        builder.HasIndex(x => x.UserName)
            .IsUnique(false);
        builder.HasIndex(x => x.NormalizedUserName)
            .IsUnique(false);
        builder.HasIndex(x => x.Email)
            .IsUnique(false);
        builder.HasIndex(x => x.NormalizedEmail)
            .IsUnique(false);

        builder.Property(x => x.FirstName)
            .HasMaxLength(32);
        builder.Property(x => x.LanguageCode)
            .HasMaxLength(2);
        builder.Property(x => x.City)
            .HasMaxLength(32);

        builder.HasMany(x => x.ChatMessages)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.HostedRooms)
            .WithOne(x => x.Host)
            .HasForeignKey(x => x.HostId)
            .IsRequired();

        builder.HasMany(x => x.Notifications)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.RoomsInUse)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.RelatedUsers)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.RelatedTo)
            .WithOne(x => x.RelatedUser)
            .HasForeignKey(x => x.RelatedUserId)
            .IsRequired();

        builder.HasMany(x => x.TeamsMember)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.UserClaims)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.UserLogins)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();

        builder.HasMany(x => x.UserTokens)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}