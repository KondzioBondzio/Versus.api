using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Versus.Shared.Auth;

namespace Versus.Api.Tests.Auth;

public class LoginHandlerTests
{
    [Theory]
    [InlineData("demo1@test.com", "Qwerty1!")]
    public async Task LoginHandler_ShouldSucceed(string login, string password)
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = factory.CreateClient();

        _ = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Login = login,
            Password = password,
            UserName = "demo",
            Gender = 0,
            YearOfBirth = 2000,
            City = "Warsaw",
            Language = "pl"
        });

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Login = login, Password = password
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result.Should().NotBe(string.Empty);
        result!.TokenType.Should().NotBe(string.Empty);
        result.AccessToken.Should().NotBe(string.Empty);
        result.RefreshToken.Should().NotBe(string.Empty);
    }

    [Fact]
    public async Task LoginHandler_ShouldFail()
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Login = string.Empty, Password = string.Empty
        });

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }
}