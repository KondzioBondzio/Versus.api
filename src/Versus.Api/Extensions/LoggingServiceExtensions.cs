using Serilog;

namespace Versus.Api.Extensions;

public static class LoggingServiceExtensions
{
    public static IHostBuilder AddLogging(this IHostBuilder host, IConfiguration configuration)
    {
        host.UseSerilog((_, services, config) =>
        {
            config.ReadFrom.Configuration(configuration)
                .ReadFrom.Services(services);
        }, writeToProviders: true);

        return host;
    }
}