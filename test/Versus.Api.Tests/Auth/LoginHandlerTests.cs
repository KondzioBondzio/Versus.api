using System.Net;
using System.Net.Http.Json;
using Versus.Shared.Auth;

namespace Versus.Api.Tests.Auth;

public class LoginHandlerTests
{
    [Theory]
    [InlineData("demo1@test.com", "Qwerty1!")]
    public async Task LoginHandler_WithValidCredentials_ShouldSucceed(string login, string password)
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = factory.CreateClient();
        
        // TODO: replace with DbContext call?
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
        var request = new LoginRequest
        {
            Login = login,
            Password = password
        };
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(string.Empty, result.TokenType);
        Assert.NotEqual(string.Empty, result.AccessToken);
        Assert.NotEqual(string.Empty, result.RefreshToken);
    }

    [Theory]
    [InlineData("", "", HttpStatusCode.BadRequest)]
    [InlineData("invalid", "password", HttpStatusCode.BadRequest)]
    [InlineData("demo1@test.com", "1", HttpStatusCode.BadRequest)]
    [InlineData("nonexistent@test.com", "ValidPass1!", HttpStatusCode.Unauthorized)]
    [InlineData("demo1@test.com", "WrongPass1!", HttpStatusCode.Unauthorized)]
    public async Task LoginHandler_WithInvalidCredentials_ShouldFail(string login, string password,
        HttpStatusCode expectedStatusCode)
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = factory.CreateClient();

        // Act
        var request = new LoginRequest
        {
            Login = login,
            Password = password
        };
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(expectedStatusCode, response.StatusCode);
    }
}