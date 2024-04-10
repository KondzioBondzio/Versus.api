﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Versus.Api.Entities;

namespace Versus.Api.Data.Configuration;

public class UserLoginConfiguration : IEntityTypeConfiguration<UserLogin>
{
    public void Configure(EntityTypeBuilder<UserLogin> builder)
    {
        builder.ToTable("UserLogins");

        builder.HasOne(x => x.User)
            .WithMany(x => x.UserLogins)
            .HasForeignKey(x => x.UserId)
            .IsRequired();
    }
}