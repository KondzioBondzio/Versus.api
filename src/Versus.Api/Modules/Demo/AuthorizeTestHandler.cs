using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Versus.Api.Modules.Demo;

public record AuthorizeTestParameters
{
    public ClaimsPrincipal User { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out ClaimsPrincipal user,
        out CancellationToken cancellationToken)
    {
        user = User;
        cancellationToken = CancellationToken;
    }
}

public static class AuthorizeTestHandler
{
    public static Results<ContentHttpResult, UnauthorizedHttpResult> Handle
        ([AsParameters] AuthorizeTestParameters parameters)
    {
        var (user, _) = parameters;
        if (!user.Identity?.IsAuthenticated ?? false)
        {
            return TypedResults.Unauthorized();
        }

        string? id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        string? email = user.FindFirstValue(ClaimTypes.Email);

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"IsAuthenticated: {user.Identity?.IsAuthenticated ?? false}");
        stringBuilder.AppendLine($"Id: {id}");
        stringBuilder.AppendLine($"Email: {email}");

        return TypedResults.Content(stringBuilder.ToString());
    }
}
