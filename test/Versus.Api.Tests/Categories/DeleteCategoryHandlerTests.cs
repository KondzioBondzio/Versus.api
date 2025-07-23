using System.Net;
using Versus.Api.Entities;

namespace Versus.Api.Tests.Categories;

public class DeleteCategoryHandlerTests
{
    [Theory]
    [InlineData("Category X")]
    public async Task DeleteCategoryHandler_ShouldNoContent(string name)
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var category = new Category
        {
            Name = name
        };
        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();
        
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);

        // Act
        var response = await client.DeleteAsync($"/api/categories/{category.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(dbContext.Categories.Any(x => x.Id == category.Id));
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
        Assert.False(dbContext.Categories.Any(x => x.Id == categoryId));
    }
    
    // TODO: test delete when category is in use by room (via seeded data?)
}