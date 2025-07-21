using System.Net;
using System.Net.Http.Json;
using Versus.Api.Entities;
using Versus.Shared.Categories;

namespace Versus.Api.Tests.Categories;

public class UpdateCategoryHandlerTests
{
    [Theory]
    [InlineData("Category X")]
    public async Task UpdateCategoryHandler_ShouldSucceed(string name)
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
        var request = new UpdateCategoryRequest
        {
            Name = name + " updated"
        };
        var response = await client.PutAsJsonAsync($"/api/categories/{category.Id}", request);

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
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);

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