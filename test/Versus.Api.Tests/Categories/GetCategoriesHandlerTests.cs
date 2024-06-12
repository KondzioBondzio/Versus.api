using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Versus.Api.Data;
using Versus.Shared.Categories;
using Versus.Shared.Common;
using Versus.Shared.Rooms;

namespace Versus.Api.Tests.Categories;

public class GetCategoriesHandlerTests
{
    [Fact]
    public async Task CreateRoomHandler_ShouldSucceed()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = await factory.CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetCategoriesResponse>>();
        result.Should().NotBeNull();
    }
}