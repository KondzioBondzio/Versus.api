using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Versus.Api.Data;
using Versus.Api.Entities;

namespace Versus.Api.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        // services.Configure<IdentityOptions>(options =>
        // {
        //     options.SignIn.RequireConfirmedEmail = false;
        // });

        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<VersusDbContext>();

        AuthenticationBuilder authenticationBuilder = services.AddAuthentication(IdentityConstants.BearerScheme)
            .AddBearerToken(IdentityConstants.BearerScheme);
        AuthorizationPolicyBuilder policyBuilder = new(IdentityConstants.BearerScheme);

        const string jwtBearerConfig = "Authentication:Schemes:JwtBearer";
        authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[$"{jwtBearerConfig}:Key"]!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = configuration[$"{jwtBearerConfig}:Audience"],
                    ValidIssuer = configuration[$"{jwtBearerConfig}:Issuer"],
                    ClockSkew = TimeSpan.Zero
                };
            });
        policyBuilder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);

        const string keyCloakConfig = "Authentication:Schemes:KeyCloak";
        if (IsSectionConfigured(configuration.GetSection(keyCloakConfig)))
        {
            authenticationBuilder.AddJwtBearer("KeyCloak", options =>
            {
                options.RequireHttpsMetadata = false;
                options.MetadataAddress = configuration[$"{keyCloakConfig}:MetadataAddress"]!;
                options.Authority = configuration[$"{keyCloakConfig}:Authority"];
                options.Audience = configuration[$"{keyCloakConfig}:Audience"];
            });
            policyBuilder.AddAuthenticationSchemes("KeyCloak");
        }

        const string googleConfig = "Authentication:Schemes:Google";
        if (IsSectionConfigured(configuration.GetSection(googleConfig)))
        {
            authenticationBuilder.AddGoogle(options =>
            {
                options.Events.OnRedirectToAuthorizationEndpoint = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api/Auth/login"))
                    {
                        context.Response.Redirect(context.RedirectUri);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }

                    return Task.CompletedTask;
                };

                options.ClientId = configuration[$"{googleConfig}:ClientId"]!;
                options.ClientSecret = configuration[$"{googleConfig}:ClientSecret"]!;
            });
            policyBuilder.AddAuthenticationSchemes(GoogleDefaults.AuthenticationScheme);
        }

        AuthorizationPolicy policy = policyBuilder.RequireAuthenticatedUser().Build();
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(policy);

        return services;
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