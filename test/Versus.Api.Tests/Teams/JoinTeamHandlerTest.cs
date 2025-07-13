using System.Net;

namespace Versus.Api.Tests.Teams;

public class JoinTeamHandlerTests
{
    [Fact]
    public async Task JoinRoomHandler_ShouldSucceed()
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);
        var teamId = dbContext.Teams.Select(x => x.Id).First();

        // Act
        var response = await client.PostAsync($"/api/teams/{teamId}/join", new StringContent(string.Empty));

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}