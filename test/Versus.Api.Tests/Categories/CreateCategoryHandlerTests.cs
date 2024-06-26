﻿using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Versus.Shared.Categories;

namespace Versus.Api.Tests.Categories;

public class CreateCategoryHandlerTests
{
    [Fact]
    public async Task CreateCategoryHandler_ShouldSucceed()
    {
        // Arrange
        await using var fixture = new WebAppFixture();
        var dbContext = fixture.DbContext;
        var user = dbContext.Users.First();
        var client = fixture.CreateAuthenticatedClient(user);

        // Act
        var request = new CreateCategoryRequest
        {
            Name = "Category test"
        };
        var response = await client.PostAsJsonAsync("/api/categories", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}