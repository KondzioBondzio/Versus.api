using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Entities;
using Versus.Api.Services.Auth;

namespace Versus.Api.Endpoints.Auth;

public class ExternalLoginCallbackHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapGet("/login/{scheme}/callback", HandleAsync)
        .AllowAnonymous();

    public static async Task<Results<ContentHttpResult, UnauthorizedHttpResult, NoContent>> HandleAsync(
        string scheme,
        HttpContext httpContext,
        ITokenService tokenService,
        CancellationToken cancellationToken)
    {
        // TODO: Handle external login callback, including creating a local user if necessary

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

        var user = new User
        {
            Id = Guid.NewGuid(), UserName = email, Email = email
        };
        string accessToken = tokenService.GenerateAccessToken(user);
        string refreshToken = tokenService.GenerateRefreshToken(user);
        stringBuilder.AppendLine($"Access Token: {accessToken}");
        stringBuilder.AppendLine($"Refresh Token: {refreshToken}");

        return TypedResults.Content(stringBuilder.ToString());
    }
}