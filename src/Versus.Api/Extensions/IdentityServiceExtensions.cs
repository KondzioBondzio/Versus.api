using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Versus.Api.Data;
using Versus.Api.Entities;
using Versus.Api.Validation.Validators;

namespace Versus.Api.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        // services.Configure<IdentityOptions>(options =>
        // {
        //     options.SignIn.RequireConfirmedEmail = false;
        // });

        services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddRoles<Role>()
            .AddSignInManager()
            .AddEntityFrameworkStores<VersusDbContext>();

        services.Replace(ServiceDescriptor.Scoped<IUserValidator<User>, IdentityUserValidator>());

        var authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

        const string jwtBearerConfig = "Authentication:Schemes:JwtBearer";
        authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
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

        const string keyCloakConfig = "Authentication:Schemes:KeyCloak";
        if (IsSectionConfigured(configuration.GetSection(keyCloakConfig)))
        {
            authBuilder.AddJwtBearer("KeyCloak", options =>
            {
                options.RequireHttpsMetadata = false;
                options.MetadataAddress = configuration[$"{keyCloakConfig}:MetadataAddress"]!;
                options.Authority = configuration[$"{keyCloakConfig}:Authority"];
                options.Audience = configuration[$"{keyCloakConfig}:Audience"];
            });
        }

        const string googleConfig = "Authentication:Schemes:Google";
        if (IsSectionConfigured(configuration.GetSection(googleConfig)))
        {
            authBuilder.AddGoogle(options =>
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
        }

        AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, "KeyCloak", GoogleDefaults.AuthenticationScheme)
            .Build();
        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(policy)
            .AddPolicy("xdd", policy);

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