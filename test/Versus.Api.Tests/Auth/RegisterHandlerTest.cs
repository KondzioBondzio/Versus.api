using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Versus.Shared.Auth;

namespace Versus.Api.Tests.Auth;

public class RegisterHandlerTests
{
    private RegisterRequest RegisterRequest1 = new RegisterRequest
    {
        Login = "demo@test.com",
        Password = "Qwerty1!",
        UserName = "demo",
        YearOfBirth = 2000,
        Gender = 0,
        Language = "pl",
        City = "Warsaw"
    };

    [Fact]
    public async Task RegisterHandler_ShouldSucceed()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", RegisterRequest1);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RegisterHandler_ShouldFail()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Login = string.Empty,
            Password = string.Empty,
            UserName = string.Empty,
            Gender = 0,
            YearOfBirth = DateTime.Today.Year,
            City = string.Empty,
            Language = string.Empty
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}