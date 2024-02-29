using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Versus.Api.Services;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Modules.Auth;

public static class LoginHandler
{
    public static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> Handle
        ([FromServices] IServiceProvider sp, LoginRequest request, CancellationToken cancellationToken)
    {
        var authService = sp.GetRequiredService<IAuthService>();
        var tokenService = sp.GetRequiredService<ITokenService>();
        var userService = sp.GetRequiredService<IUserService>();

        var user = await userService.FindByEmailAsync(request.Login, cancellationToken);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        bool result = await authService.ValidateCredentialsAsync(user, request.Password);
        if (!result)
        {
            return TypedResults.Unauthorized();
        }

        string accessToken = tokenService.GenerateAccessToken(user);
        string refreshToken = tokenService.GenerateRefreshToken(user);

        return TypedResults.Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
}
