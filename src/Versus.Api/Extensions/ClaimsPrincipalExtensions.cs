using System.Security.Claims;

namespace Versus.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal claimsPrincipal)
    {
        var claimId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(claimId, out var id))
        {
            throw new UnauthorizedAccessException();
        }

        return id;
    }
}