using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Versus.Api.Data;
using Versus.Shared.Rooms;

namespace Versus.Api.Tests.Rooms;

public class JoinRoomHandlerTests
{
    [Fact]
    public async Task JoinRoomHandler_ShouldSucceed()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = await factory.CreateAuthenticatedClient();

        var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<VersusDbContext>();
        var room = dbContext.Rooms.First();

        // Act
        var id = room.Id;
        var request = new JoinRoomRequest
        {
            Password = room.Password
        };
        var response = await client.PostAsJsonAsync($"/api/rooms/{id}/join", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}