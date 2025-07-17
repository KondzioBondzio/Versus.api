using Microsoft.AspNetCore.Authorization;

namespace Versus.Api.Authorization;

public record PermissionRequirement(string Permission) : IAuthorizationRequirement;