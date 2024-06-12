using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Versus.Shared.Categories;

namespace Versus.Api.Tests.Categories;

public class CreateCategoryHandlerTests
{
    [Fact]
    public async Task CreateRoomHandler_ShouldSucceed()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = await factory.CreateAuthenticatedClient();

        // Act
        var request = new CreateCategoryRequest
        {
            Name = "Test room", 
            Description = string.Empty,
            Image = null
        };
        var response = await client.PostAsJsonAsync("/api/categories", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}