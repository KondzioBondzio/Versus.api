using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Versus.Api.Controllers;
using Versus.Core.Features.Weather;
using Versus.Domain;
using Versus.Domain.Entities;

namespace Versus.Host;

public static class VersusContext
{
    public static WebApplicationBuilder AddVersusContext(this WebApplicationBuilder builder)
    {
        builder.AddDatabase();
        builder.AddAuth();

        Assembly apiAssembly = typeof(WeatherForecastController).Assembly;
        builder.Services.AddControllers()
            .AddApplicationPart(apiAssembly);

        Assembly coreAssembly = typeof(GetForecast).Assembly;
        builder.Services.AddMediatR(options =>
            options.RegisterServicesFromAssembly(coreAssembly));

        builder.AddLogging();

        return builder;
    }

    private static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        // builder.Services.AddDbContext<VersusDbContext>(options => options.UseInMemoryDatabase("Versus"));

        builder.Services.AddDbContext<VersusDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default"),
                opt =>
                {
                    string? migrationsAssemblyName = typeof(VersusContext).Assembly.GetName().Name;
                    opt.MigrationsAssembly(migrationsAssemblyName);
                }));

        return builder;
    }

    private static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        AuthenticationBuilder authenticationBuilder = builder.Services
            .AddAuthentication(IdentityConstants.BearerScheme);
        AuthorizationPolicyBuilder authorizationPolicyBuilder = new(IdentityConstants.BearerScheme);

        builder.AddIdentity(authenticationBuilder, authorizationPolicyBuilder);
        builder.AddKeyCloak(authenticationBuilder, authorizationPolicyBuilder);

        AuthorizationPolicy policy = authorizationPolicyBuilder.RequireAuthenticatedUser().Build();
        builder.Services.AddAuthorizationBuilder()
            .SetDefaultPolicy(policy);

        return builder;
    }

    private static AuthenticationBuilder AddIdentity(this WebApplicationBuilder builder,
        AuthenticationBuilder authBuilder,
        AuthorizationPolicyBuilder policyBuilder)
    {
        authBuilder.AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<VersusDbContext>()
            .AddApiEndpoints();

        policyBuilder.AddAuthenticationSchemes(IdentityConstants.BearerScheme);

        return authBuilder;
    }

    private static AuthenticationBuilder AddKeyCloak(this WebApplicationBuilder builder,
        AuthenticationBuilder authBuilder,
        AuthorizationPolicyBuilder policyBuilder)
    {
        IConfiguration config = builder.Configuration;
        const string baseSelector = "Authentication:Schemes:KeyCloak";
        authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.RequireHttpsMetadata = false;
            options.MetadataAddress = config[$"{baseSelector}:MetadataAddress"] ?? string.Empty;
            options.Authority = config[$"{baseSelector}:Authority"];
            options.Audience = config[$"{baseSelector}:Audience"];
        });

        policyBuilder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);

        return authBuilder;
    }

    private static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(builder.Configuration)
                .ReadFrom.Services(services);
        }, writeToProviders: true);

        return builder;
    }
}
