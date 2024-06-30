﻿using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Versus.Shared.Categories;
using Versus.Shared.Common;

namespace Versus.Api.Tests.Categories;

public class GetCategoriesHandlerTests
{
    [Fact]
    public async Task GetCategoriesHandler_ShouldSucceed()
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);

        // Act
        var response = await client.GetAsync("/api/categories");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<PagedResponse<GetCategoriesResponse>>();
        result.Should().NotBeNull();
    }
}