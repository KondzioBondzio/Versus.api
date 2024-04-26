using System.Net;
using System.Net.Http.Json;
using Versus.Shared.Auth;

namespace Versus.Api.Tests.Auth;

public class LoginHandlerTests
{
    [Theory]
    [InlineData("demo@test.com", "Qwerty1!")]
    public async Task LoginHandler_ShouldSucceed(string login, string password)
    {
        // Arrange
        await using var factory = new WebAppFixture();
        var client = factory.CreateClient();

        await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = login, Password = password, Login = login
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
        Assert.NotEmpty(result.TokenType);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
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
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}