using Microsoft.AspNetCore.Authorization;
using Versus.Api.Authorization;
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
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();


        services.AddTransient<VersusMigrator>();

        return services;
    }

    public static IServiceCollection AddPolicyAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder();

        return services;
    }

    private static AuthorizationBuilder AddPermissionPolicy(this AuthorizationBuilder builder, string name)
    {
        return builder.AddPermissionPolicy(name, name);
    }

    private static AuthorizationBuilder AddPermissionPolicy(this AuthorizationBuilder builder, string name,
        string permissionName)
    {
        return builder.AddPolicy(name, policy => policy.Requirements.Add(new PermissionRequirement(permissionName)));
    }
}