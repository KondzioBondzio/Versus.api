using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using Versus.Api.Data;
using Versus.Api.Entities;

namespace Versus.Api.Features;

public class DatabaseFeatureDefinitionProvider : IFeatureDefinitionProvider
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseFeatureDefinitionProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IAsyncEnumerable<FeatureDefinition> GetAllFeatureDefinitionsAsync()
    {
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VersusDbContext>();
        return dbContext.Features
            .AsNoTracking()
            .Select(x => CreateFeatureDefinition(x.Name, x))
            .AsAsyncEnumerable();
    }

    public async Task<FeatureDefinition> GetFeatureDefinitionAsync(string featureName)
    {
        var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VersusDbContext>();
        var feature = await dbContext.Features
            .Include(x => x.Filters)
            .ThenInclude(x => x.Parameters)
            .AsNoTracking()
            .Where(x => x.Name == featureName)
            .SingleOrDefaultAsync();

        return CreateFeatureDefinition(featureName, feature);
    }

    private static FeatureDefinition CreateFeatureDefinition(string featureName, Feature? feature)
    {
        return new FeatureDefinition
        {
            Name = featureName,
            EnabledFor = feature?.Filters
                             .Select(x => new FeatureFilterConfiguration
                             {
                                 Name = x.Name,
                                 Parameters = new ConfigurationBuilder()
                                     .AddInMemoryCollection(x.Parameters
                                         .ToDictionary(p => p.Name, p => p.Value)!)
                                     .Build()
                             })
                         ?? new List<FeatureFilterConfiguration>()
        };
    }
}