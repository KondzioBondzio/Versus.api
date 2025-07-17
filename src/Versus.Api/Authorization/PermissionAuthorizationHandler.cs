using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Versus.Api.Extensions;
using Versus.Api.Services.Auth;

namespace Versus.Api.Authorization;

public class PermissionAuthorizationHandler(IPermissionService permissionService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var user = context.User;
        var idClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(idClaim))
        {
            return;
        }

        var userId = context.User.GetUserId();
        var hasPermission = await permissionService.HasPermissionAsync(userId, requirement.Permission);
        if (hasPermission)
        {
            context.Succeed(requirement);
        }
    }
}