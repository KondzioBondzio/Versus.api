using System.Net;
using Microsoft.EntityFrameworkCore;

namespace Versus.Api.Tests.Categories;

public class DeleteCategoryHandlerTests
{
    [Fact]
    public async Task DeleteCategoryHandler_ShouldNoContent()
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);
        var categoryId = dbContext.Categories
            .Include(x => x.Rooms)
            .Where(x => x.Rooms.Count == 0)
            .Select(x => x.Id)
            .First();

        // Act
        var response = await client.DeleteAsync($"/api/categories/{categoryId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCategoryHandler_ShouldNotFound()
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);
        var categoryId = Guid.NewGuid();

        // Act
        var response = await client.DeleteAsync($"/api/categories/{categoryId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}