using System.Reflection;
using Microsoft.AspNetCore.Authentication;
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
using Versus.Host.Migrations;

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

        builder.Services.AddTransient<VersusMigrator>();

        return builder;
    }

    private static WebApplicationBuilder AddAuth(this WebApplicationBuilder builder)
    {
        AuthenticationBuilder authenticationBuilder = builder.Services
            .AddAuthentication(IdentityConstants.BearerScheme);
        AuthorizationPolicyBuilder authorizationPolicyBuilder = new(IdentityConstants.BearerScheme);

        builder.AddIdentity(authenticationBuilder, authorizationPolicyBuilder);
        builder.AddKeyCloak(authenticationBuilder, authorizationPolicyBuilder);
        builder.AddSocialAuth(authenticationBuilder, authorizationPolicyBuilder);

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
            .AddEntityFrameworkStores<VersusDbContext>();

        policyBuilder.AddAuthenticationSchemes(IdentityConstants.BearerScheme);

        return authBuilder;
    }

    private static AuthenticationBuilder AddKeyCloak(this WebApplicationBuilder builder,
        AuthenticationBuilder authBuilder,
        AuthorizationPolicyBuilder policyBuilder)
    {
        ConfigurationManager config = builder.Configuration;
        const string keyCloakConfig = "Authentication:Schemes:KeyCloak";
        if (IsSectionConfigured(config.GetSection(keyCloakConfig)))
        {
            authBuilder.AddJwtBearer("KeyCloak", options =>
            {
                options.RequireHttpsMetadata = false;
                options.MetadataAddress = config[$"{keyCloakConfig}:MetadataAddress"] ?? string.Empty;
                options.Authority = config[$"{keyCloakConfig}:Authority"];
                options.Audience = config[$"{keyCloakConfig}:Audience"];
            });
            policyBuilder.AddAuthenticationSchemes("KeyCloak");
        }

        return authBuilder;
    }

    private static AuthenticationBuilder AddSocialAuth(this WebApplicationBuilder builder,
        AuthenticationBuilder authBuilder,
        AuthorizationPolicyBuilder policyBuilder)
    {
        ConfigurationManager config = builder.Configuration;

        const string googleConfig = "Authentication:Schemes:Google";
        if (IsSectionConfigured(config.GetSection(googleConfig)))
        {
            authBuilder.AddGoogle("Google", options =>
            {
                options.ClientId = config[$"{googleConfig}:ClientId"]!;
                options.ClientSecret = config[$"{googleConfig}:ClientSecret"]!;
            });
            policyBuilder.AddAuthenticationSchemes("Google");
        }

        const string facebookConfig = "Authentication:Schemes:Facebook";
        if (IsSectionConfigured(config.GetSection(facebookConfig)))
        {
            authBuilder.AddFacebook("Facebook", options =>
            {
                options.AppId = config[$"{facebookConfig}:AppId"]!;
                options.AppSecret = config[$"{facebookConfig}:AppSecret"]!;
            });
            policyBuilder.AddAuthenticationSchemes("Facebook");
        }

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

    private static bool IsSectionConfigured(IConfigurationSection? section)
    {
        if (!section.Exists())
        {
            return false;
        }

        return section.GetChildren().All(child => !string.IsNullOrEmpty(child.Value));
    }
}
