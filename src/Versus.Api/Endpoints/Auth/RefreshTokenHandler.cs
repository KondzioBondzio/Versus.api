using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Extensions;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Endpoints.Auth;

public class RefreshTokenHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/refresh-token", HandleAsync)
        .AllowAnonymous()
        .WithRequestValidation<RefreshTokenRequest>();

    public static async Task<Results<Ok<RefreshTokenResponse>, UnauthorizedHttpResult>> HandleAsync(
        RefreshTokenRequest request,
        IUserService userService,
        ITokenService tokenService,
        CancellationToken cancellationToken)
    {
        if (!tokenService.IsTokenValid(request.Token))
        {
            return TypedResults.Unauthorized();
        }

        ClaimsPrincipal principal = tokenService.ReadToken(request.Token);
        string id = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var user = await userService.FindByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        string accessToken = tokenService.GenerateAccessToken(user);
        string refreshToken = tokenService.GenerateRefreshToken(user);

        return TypedResults.Ok(new RefreshTokenResponse
        {
            AccessToken = accessToken, RefreshToken = refreshToken
        });
    }
}