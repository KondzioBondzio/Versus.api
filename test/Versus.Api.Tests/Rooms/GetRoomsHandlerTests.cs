using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Versus.Api.Tests.Helpers;
using Versus.Shared.Common;
using Versus.Shared.Rooms;

namespace Versus.Api.Tests.Rooms;

public class GetRoomHandlerTests
{
    [Fact]
    public async Task GetRoomsHandler_ShouldSucceed()
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);

        // Act
        var request = new GetRoomsRequest
        {
            PageSize = 10
        };
        var response = await client.GetAsync($"/api/rooms?{request.AsQueryString()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetRoomsResponse>>();
        result.Should().NotBeNull();
        result!.Data.Should().HaveCountLessOrEqualTo(10);
    }
}