using System.Net;
using System.Net.Http.Json;
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
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(string.Empty, result.TokenType);
        Assert.NotEqual(string.Empty, result.AccessToken);
        Assert.NotEqual(string.Empty, result.RefreshToken);
    }

    [Fact]
    public async Task LoginHandler_ShouldFailRequestValidation()
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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}