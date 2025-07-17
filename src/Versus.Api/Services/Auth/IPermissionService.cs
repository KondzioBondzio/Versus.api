namespace Versus.Api.Services.Auth;

public interface IPermissionService
{
    Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> HasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default);
}