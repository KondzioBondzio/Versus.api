using Microsoft.AspNetCore.Http.HttpResults;
using Versus.Api.Extensions;
using Versus.Api.Services.Auth;
using Versus.Shared.Auth;

namespace Versus.Api.Endpoints.Auth;

public record LoginParameters
{
    public LoginRequest Request { get; init; } = default!;
    public IAuthService AuthService { get; init; } = default!;
    public ITokenService TokenService { get; init; } = default!;
    public IUserService UserService { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out LoginRequest request,
        out IAuthService authService,
        out ITokenService tokenService,
        out IUserService userService,
        out CancellationToken cancellationToken)
    {
        request = Request;
        authService = AuthService;
        tokenService = TokenService;
        userService = UserService;
        cancellationToken = CancellationToken;
    }
}

public class LoginHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapPost("/login", HandleAsync)
        .AllowAnonymous()
        .WithRequestValidation<LoginParameters, LoginRequest>();

    public static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult>> HandleAsync
        ([AsParameters] LoginParameters parameters)
    {
        var (request, authService, tokenService, userService, cancellationToken) = parameters;

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