using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Modules.Auth;

public static class RefreshTokenHandler
{
    public static async Task<Results<Ok<RefreshTokenResponse>, UnauthorizedHttpResult>> Handle
        ([FromServices] IServiceProvider sp, RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var userService = sp.GetRequiredService<IUserService>();
        var tokenService = sp.GetRequiredService<ITokenService>();

        if (!tokenService.IsTokenValid(request.Token))
        {
            return TypedResults.Unauthorized();
        }

        ClaimsPrincipal principal = tokenService.ReadToken(request.Token);
        string id = principal.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier)
            .Select(x => x.Value)
            .FirstOrDefault() ?? string.Empty;
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
