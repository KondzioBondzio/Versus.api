using Versus.Api.Migrations;
using Versus.Api.Services;

namespace Versus.Api.Extensions;

public static class VersusServiceExtensions
{
    public static IServiceCollection AddVersusServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ITokenService, JwtTokenService>();

        services.AddTransient<VersusMigrator>();

        return services;
    }
}
