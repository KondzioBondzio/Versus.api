using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Versus.Api.Data;
using Versus.Shared.Common;

namespace Versus.Api.Tests.Categories;

public class DeleteCategoryHandlerTests
{
    [Fact]
    public async Task CreateRoomHandler_ShouldNoContent()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = await factory.CreateAuthenticatedClient();

        var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VersusDbContext>();

        // Act
        var id = dbContext.Categories
            .Include(x => x.Rooms)
            .Where(x => x.Rooms.Count == 0)
            .Select(x => x.Id)
            .First();
        var response = await client.DeleteAsync($"/api/categories/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task CreateRoomHandler_ShouldNotFound()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = await factory.CreateAuthenticatedClient();

        // Act
        var id = Guid.NewGuid();
        var response = await client.DeleteAsync($"/api/categories/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task CreateRoomHandler_ShouldConflict()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = await factory.CreateAuthenticatedClient();

        var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VersusDbContext>();

        // Act
        var id = dbContext.Categories
            .Include(x => x.Rooms)
            .Where(x => x.Rooms.Count != 0)
            .Select(x => x.Id)
            .First();
        var response = await client.DeleteAsync($"/api/categories/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var result = await response.Content.ReadFromJsonAsync<ConflictResponse>();
        result.Should().NotBeNull();
        result!.Errors.Should().NotBeEmpty();
    }
}