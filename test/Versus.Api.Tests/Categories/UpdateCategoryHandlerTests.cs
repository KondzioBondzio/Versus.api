using System.Net;
using System.Net.Http.Json;
using Versus.Api.Entities;
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
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Theory]
    [InlineData("Category X")]
    public async Task UpdateCategoryHandler_ShouldFailRequestValidation(string name)
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);
        var category1 = new Category
        {
            Name = name 
        };
        var category2 = new Category
        {
            Name = name + " copy"
        };
        await dbContext.Categories.AddRangeAsync(category1, category2);
        await dbContext.SaveChangesAsync();

        // Act
        var request = new UpdateCategoryRequest
        {
            Name = name
        };
        var response = await client.PutAsJsonAsync($"/api/categories/{category2.Id}", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}