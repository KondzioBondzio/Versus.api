using Versus.Api.Configuration;
using Versus.Api.Migrations;
using Versus.Api.Services.Auth;

namespace Versus.Api.Extensions;

public static class VersusServiceExtensions
{
    public static IServiceCollection AddVersusServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtTokenConfiguration>(configuration.GetSection("Authentication:Schemes:JwtBearer"));
        
        services.AddScoped<IUserService, EfUserService>();
        services.AddScoped<IAuthService, EfAuthService>();
        services.AddScoped<ITokenService, JwtTokenService>();

        services.AddTransient<VersusMigrator>();

        return services;
    }
}