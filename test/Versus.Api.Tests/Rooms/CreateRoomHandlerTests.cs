using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Versus.Shared.Rooms;

namespace Versus.Api.Tests.Rooms;

public class CreateRoomHandlerTests
{
    [Fact]
    public async Task CreateRoomHandler_ShouldSucceed()
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);

        // Act
        var request = new CreateRoomRequest
        {
            Name = "Room test",
            Description = "Room test description",
            PlayDate = DateTime.Today.AddDays(7),
            CategoryId = dbContext.Categories.Select(x => x.Id).First(),
            TeamCount = 2,
            TeamPlayerLimit = 5
        };
        var response = await client.PostAsJsonAsync("/api/rooms", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}