﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.ToTable("UserTokens");

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserTokens)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}