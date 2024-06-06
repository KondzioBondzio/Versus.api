using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Extensions;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Endpoints.Auth;

public class LoginHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/login", HandleAsync)
        .AllowAnonymous()
        .WithRequestValidation<LoginRequest>();

    public static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> HandleAsync(
        LoginRequest request,
        IAuthService authService,
        ITokenService tokenService,
        IUserService userService,
        CancellationToken cancellationToken)
    {
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
            AccessToken = accessToken, RefreshToken = refreshToken
        });
    }
}