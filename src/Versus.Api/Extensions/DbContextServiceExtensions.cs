using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;

namespace Versus.Api.Extensions;

public static class DbContextServiceExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<VersusDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default"),
                opt =>
                {
                    string? migrationsAssemblyName = typeof(DbContextServiceExtensions).Assembly.GetName().Name;
                    opt.MigrationsAssembly(migrationsAssemblyName);
                }));

        return services;
    }
}