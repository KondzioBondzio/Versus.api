using Microsoft.FeatureManagement;
using Versus.Api.Features;

namespace Versus.Api.Extensions;

public static class FeatureManagementExtensions
{
    public static IServiceCollection AddVersusFeatureManagement(this IServiceCollection services)
    {
        services.AddSingleton<IFeatureDefinitionProvider, DatabaseFeatureDefinitionProvider>();
        services.AddFeatureManagement();

        return services;
    }
}