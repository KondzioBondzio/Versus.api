using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Versus.Api.Modules.Auth;

public static class ExternalLoginCallbackHandler
{
    public static async Task<Results<ContentHttpResult, UnauthorizedHttpResult, NoContent>> Handle
        (HttpContext context, string scheme, CancellationToken cancellationToken)
    {
        // Handle external login callback, including creating a local user if necessary
        var auth = await context.AuthenticateAsync(scheme);
        if (!auth.Succeeded || auth.Principal == null)
        {
            return TypedResults.Unauthorized();
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"{auth.Principal.Identity?.IsAuthenticated ?? false}");
        stringBuilder.Append(" | ");
        stringBuilder.Append($"{FindClaim(auth.Principal.Claims, ClaimTypes.NameIdentifier)}");
        stringBuilder.Append(" | ");
        stringBuilder.Append($"{FindClaim(auth.Principal.Claims, ClaimTypes.Email)}");
        return TypedResults.Content(stringBuilder.ToString());
    }

    private static string? FindClaim(IEnumerable<Claim> claims, string claimType)
    {
        return claims.Where(x => x.Type == claimType)
            .Select(x => x.Value)
            .FirstOrDefault();
    }
}
