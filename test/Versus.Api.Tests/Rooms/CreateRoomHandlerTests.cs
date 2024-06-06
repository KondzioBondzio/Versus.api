using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Versus.Api.Data;
using Versus.Shared.Rooms;

namespace Versus.Api.Tests.Rooms;

public class CreateRoomHandlerTests
{
    [Fact]
    public async Task CreateRoomHandler_ShouldSucceed()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = await factory.CreateAuthenticatedClient();

        var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VersusDbContext>();

        // Act
        var request = new CreateRoomRequest
        {
            Name = "Test room", 
            CategoryId = dbContext.Categories.Select(x => x.Id).First(),
            TeamCount = 2,
            TeamPlayerLimit = 5
        };
        var response = await client.PostAsJsonAsync("/api/rooms", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}