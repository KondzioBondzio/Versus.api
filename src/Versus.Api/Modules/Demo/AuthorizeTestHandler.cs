using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Versus.Api.Modules.Demo;

public static class AuthorizeTestHandler
{
    public static Results<ContentHttpResult, UnauthorizedHttpResult> Handle
        (HttpContext context, CancellationToken cancellationToken)
    {
        var user = context.User;
        if (!user.Identity?.IsAuthenticated ?? false)
        {
            return TypedResults.Unauthorized();
        }

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"{user.Identity?.IsAuthenticated ?? false}");
        stringBuilder.Append(" | ");
        stringBuilder.Append($"{user.FindFirstValue(ClaimTypes.NameIdentifier)}");
        stringBuilder.Append(" | ");
        stringBuilder.Append($"{user.FindFirstValue(ClaimTypes.Email)}");
        return TypedResults.Content(stringBuilder.ToString());
    }
}
