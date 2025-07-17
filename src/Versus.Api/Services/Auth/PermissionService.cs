using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;

namespace Versus.Api.Services.Auth;

public class PermissionService : IPermissionService
{
    private readonly VersusDbContext _dbContext;

    public PermissionService(VersusDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var permissions = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .SelectMany(x => x.UserRoles)
            .SelectMany(x => x.Role.RolePermissions)
            .Select(x => x.Permission.Code)
            .Distinct()
            .ToArrayAsync(cancellationToken);
        return permissions;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsAsync(userId, cancellationToken);
        return permissions.Contains(permissionName);
    }
}