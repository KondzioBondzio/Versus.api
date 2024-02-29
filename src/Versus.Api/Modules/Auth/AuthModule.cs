using Versus.Api.Services;
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

        routeGroup.MapPost("/login", LoginHandler.Handle);
        routeGroup.MapPost("/register", RegisterHandler.Handle);
        routeGroup.MapPost("/refresh-token", RefreshTokenHandler.Handle);
        routeGroup.MapGet("/login/{scheme}", ExternalLoginHandler.Handle);
        routeGroup.MapGet("/login/{scheme}/callback", ExternalLoginCallbackHandler.Handle);

        return routeGroup;
    }
}
