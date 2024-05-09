using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Versus.Api.Entities;

namespace Versus.Api.Data;

public class VersusDbContext : IdentityDbContext<User, Role, Guid,
    UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<FeatureFilter> FeatureFilters => Set<FeatureFilter>();
    public DbSet<FeatureFilterParameter> FeatureFilterParameters => Set<FeatureFilterParameter>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<RoomChatMessage> RoomChatMessages => Set<RoomChatMessage>();
    public DbSet<RoomUser> RoomUsers => Set<RoomUser>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamUser> TeamUsers => Set<TeamUser>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<UserRelationship> UserRelationships => Set<UserRelationship>();


    public VersusDbContext(DbContextOptions<VersusDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VersusDbContext).Assembly);
    }
}