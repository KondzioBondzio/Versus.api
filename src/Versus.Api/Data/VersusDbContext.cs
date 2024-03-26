using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Entities;

namespace Versus.Api.Data;

public class VersusDbContext : IdentityDbContext<User, Role, Guid,
    UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public DbSet<UserRelationship> UserRelationships => Set<UserRelationship>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public VersusDbContext(DbContextOptions<VersusDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region P

        modelBuilder.Entity<Permission>(b =>
        {
            b.ToTable("Permissions");
            b.HasMany(x => x.PermissionRoles)
                .WithOne(x => x.Permission)
                .HasForeignKey(x => x.PermissionId)
                .IsRequired();
        });

        #endregion

        #region R

        modelBuilder.Entity<Role>(b =>
        {
            b.ToTable("Roles");
            b.HasMany(x => x.RoleUsers)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .IsRequired();

            b.HasMany(x => x.RoleClaims)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .IsRequired();

            b.HasMany(x => x.RolePermissions)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .IsRequired();
        });

        modelBuilder.Entity<RoleClaim>(b => { b.ToTable("RoleClaims"); });

        modelBuilder.Entity<RolePermission>(b => { b.ToTable("RolePermissions"); });

        #endregion

        #region U

        modelBuilder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.HasMany(x => x.UserClaims)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            b.HasMany(x => x.UserLogins)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            b.HasMany(x => x.UserTokens)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            b.HasMany(x => x.UserRoles)
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId)
                .IsRequired();
        });

        modelBuilder.Entity<UserClaim>(b => { b.ToTable("UserClaims"); });

        modelBuilder.Entity<UserLogin>(b => { b.ToTable("UserLogins"); });

        modelBuilder.Entity<UserRole>(b => { b.ToTable("UserRoles"); });

        modelBuilder.Entity<UserToken>(b => { b.ToTable("UserTokens"); });

        #endregion
    }
}