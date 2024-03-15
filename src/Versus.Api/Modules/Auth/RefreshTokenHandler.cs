using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Modules.Auth;

public record RefreshTokenParameters
{
    public RefreshTokenRequest Request { get; init; } = default!;
    public IUserService UserService { get; init; } = default!;
    public ITokenService TokenService { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out RefreshTokenRequest request,
        out IUserService userService,
        out ITokenService tokenService,
        out CancellationToken cancellationToken)
    {
        request = Request;
        userService = UserService;
        tokenService = TokenService;
        cancellationToken = CancellationToken;
    }
}

public static class RefreshTokenHandler
{
    public static async Task<Results<Ok<RefreshTokenResponse>, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] RefreshTokenParameters parameters)
    {
        var (request, userService, tokenService, cancellationToken) = parameters;

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
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}
