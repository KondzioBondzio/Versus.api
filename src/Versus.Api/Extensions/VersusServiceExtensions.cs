using Versus.Api.Configuration;
using Versus.Api.Migrations;

namespace Versus.Api.Extensions;

public static class VersusServiceExtensions
{
    public static IServiceCollection AddVersusServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtTokenConfiguration>(configuration.GetSection("Authentication:Schemes:JwtBearer"));

        services.AddTransient<VersusMigrator>();

        return services;
    }
}