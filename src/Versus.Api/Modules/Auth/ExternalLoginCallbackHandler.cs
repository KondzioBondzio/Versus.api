using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Versus.Api.Modules.Auth;

public record ExternalLoginCallbackParameters
{
    public HttpContext HttpContext { get; init; } = default!;
    public string Scheme { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out HttpContext httpContext,
        out string scheme,
        out CancellationToken cancellationToken)
    {
        httpContext = HttpContext;
        scheme = Scheme;
        cancellationToken = CancellationToken;
    }
}

public static class ExternalLoginCallbackHandler
{
    public static async Task<Results<ContentHttpResult, UnauthorizedHttpResult, NoContent>> HandleAsync
        ([AsParameters] ExternalLoginCallbackParameters parameters)
    {
        // TODO: Handle external login callback, including creating a local user if necessary

        var (httpContext, scheme, _) = parameters;

        var auth = await httpContext.AuthenticateAsync(scheme);
        if (!auth.Succeeded || auth.Principal == null)
        {
            return TypedResults.Unauthorized();
        }

        string? id = auth.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
        string? email = auth.Principal.FindFirstValue(ClaimTypes.Email);

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"IsAuthenticated: {auth.Principal.Identity?.IsAuthenticated ?? false}");
        stringBuilder.AppendLine($"Id: {id}");
        stringBuilder.AppendLine($"Email: {email}");

        return TypedResults.Content(stringBuilder.ToString());
    }
}
