using Versus.Api.Services.Auth;

namespace Versus.Api.Modules.Auth;

[EndpointGroupName("Auth")]
public class AuthModule : IModule
{
    public IServiceCollection RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IUserService, EfUserService>();
        services.AddScoped<IAuthService, EfAuthService>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup(string.Empty)
            .AllowAnonymous();

        routeGroup.MapPost("/login", LoginHandler.HandleAsync);
        routeGroup.MapPost("/register", RegisterHandler.HandleAsync);
        routeGroup.MapPost("/refresh-token", RefreshTokenHandler.HandleAsync);
        routeGroup.MapGet("/login/{scheme}", ExternalLoginHandler.Handle);
        routeGroup.MapGet("/login/{scheme}/callback", ExternalLoginCallbackHandler.HandleAsync);

        return routeGroup;
    }
}
