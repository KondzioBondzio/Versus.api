using Microsoft.EntityFrameworkCore;
using Versus.Api.Data;
using Versus.Api.Data.Interceptors;

namespace Versus.Api.Extensions;

public static class DbContextServiceExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextPool<VersusDbContext>((provider, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Default"), opt =>
            {
                string? migrationsAssemblyName = typeof(DbContextServiceExtensions).Assembly.GetName().Name;
                opt.MigrationsAssembly(migrationsAssemblyName);
            });
            
            using var scope = provider.CreateScope();
            var sessionProvider = scope.ServiceProvider.GetRequiredService<ICurrentSessionProvider>();
            options.AddInterceptors(new AuditInterceptor(sessionProvider));
        });

        return services;
    }
}