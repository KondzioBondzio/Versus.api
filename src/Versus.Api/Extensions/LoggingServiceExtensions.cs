using Serilog;

namespace Versus.Api.Extensions;

public static class LoggingServiceExtensions
{
    public static IHostBuilder AddLogging(this IHostBuilder host, IConfiguration configuration)
    {
        host.ConfigureServices(services =>
        {
            services.AddLogging(y => y.ClearProviders());
        });

        host.UseSerilog((_, services, config) =>
        {
            config.ReadFrom.Configuration(configuration)
                .ReadFrom.Services(services);
        }, writeToProviders: true, preserveStaticLogger: true);

        return host;
    }
}