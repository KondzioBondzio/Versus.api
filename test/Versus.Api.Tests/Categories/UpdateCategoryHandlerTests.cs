using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Versus.Shared.Categories;

namespace Versus.Api.Tests.Categories;

public class UpdateCategoryHandlerTests
{
    [Fact]
    public async Task UpdateCategoryHandler_ShouldSucceed()
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);
        var categoryId = dbContext.Categories.Select(x => x.Id).First();

        // Act
        var request = new UpdateCategoryRequest
        {
            Name = "Category test updated"
        };
        var response = await client.PutAsJsonAsync($"/api/categories/{categoryId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}