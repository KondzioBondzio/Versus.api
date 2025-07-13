using System.Net;
using System.Net.Http.Json;
using Versus.Shared.Rooms;

namespace Versus.Api.Tests.Rooms;

public class JoinRoomHandlerTests
{
    [Fact]
    public async Task JoinRoomHandler_ShouldSucceed()
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);
        var room = dbContext.Rooms.First();

        // Act
        var id = room.Id;
        var request = new JoinRoomRequest
        {
            Password = room.Password
        };
        var response = await client.PostAsJsonAsync($"/api/rooms/{id}/join", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}