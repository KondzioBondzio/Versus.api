using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Versus.Api.Data;
using Versus.Api.Entities;

namespace Versus.Api.Tests.Features;

public class FeatureManagementTests
{
    [Fact]
    public async Task FeatureManagement_FeatureAlwaysOnShouldBeEnabled()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        _ = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VersusDbContext>();
        var alwaysOnFeature = new Feature
        {
            Name = "versus.feature.demoEnabled",
            Filters = new List<FeatureFilter>
            {
                new()
                {
                    Name = "AlwaysOn"
                }
            }
        };
        await dbContext.Features.AddAsync(alwaysOnFeature);
        await dbContext.SaveChangesAsync();

        // Act
        var featureManager = factory.Services.GetRequiredService<IFeatureManager>();
        var isEnabled = await featureManager.IsEnabledAsync("versus.feature.demoEnabled");

        // Assert
        Assert.True(isEnabled);
    }

    [Fact]
    public async Task FeatureManagement_FeatureDisabledShouldBeDisabled()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        _ = factory.CreateClient();

        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VersusDbContext>();
        var disabledFeature = new Feature
        {
            Name = "versus.feature.demoDisable"
        };
        await dbContext.Features.AddAsync(disabledFeature);
        await dbContext.SaveChangesAsync();

        // Act
        var featureManager = factory.Services.GetRequiredService<IFeatureManager>();
        var isEnabled = await featureManager.IsEnabledAsync("versus.feature.demoDisabled");

        // Assert
        Assert.False(isEnabled);
    }
}