using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Versus.Core.Features.Weather;
using Versus.Domain;

namespace Versus.Host;

public static class VersusContext
{
    public static WebApplicationBuilder AddVersusContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<VersusDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default"),
                opt =>
                {
                    string? migrationsAssemblyName = typeof(VersusContext).Assembly.GetName().Name;
                    opt.MigrationsAssembly(migrationsAssemblyName);
                }));

        var coreAssembly = typeof(GetForecast).Assembly;
        builder.Services.AddMediatR(options =>
            options.RegisterServicesFromAssembly(coreAssembly));

        builder.Host.UseSerilog((_, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services);
        }, writeToProviders: true);

        return builder;
    }
}
