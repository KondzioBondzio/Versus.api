using System.Net;
using System.Net.Http.Json;
using Versus.Api.Entities;
using Versus.Shared.Categories;

namespace Versus.Api.Tests.Categories;

public class GetCategoryHandlerTest
{
    [Theory]
    [InlineData("Category")]
    public async Task GetCategoryHandler_ShouldSucceed(string name)
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
        var response = await client.GetAsync($"/api/categories/{category.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseCategory = await response.Content.ReadFromJsonAsync<GetCategoryResponse>();
        Assert.NotNull(responseCategory);
        Assert.Equal(category.Id, responseCategory.Id);
        Assert.Equal(category.Name, responseCategory.Name);
    }
}