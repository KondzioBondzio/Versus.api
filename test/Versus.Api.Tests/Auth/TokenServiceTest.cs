using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Versus.Api.Configuration;
using Versus.Api.Entities;
using Versus.Api.Services.Auth;

namespace Versus.Api.Tests.Auth;

public class TokenServiceTest
{
    private readonly JwtTokenService _tokenService;

    private readonly User _user = new()
    {
        Id = Guid.NewGuid(), UserName = "demo", Email = "demo@test.com"
    };

    public TokenServiceTest()
    {
        var jwtConfiguration = new JwtTokenConfiguration
        {
            Key =
                "OWAFnYvWTHdlKGtaWGypUSgKOOWAFnYvWTHdlKGtaWGypUSgKOOWAFnYvWTHdlKGtaWGypUSgKOOWAFnYvWTHdlKGtaWGypUSgKO",
            Audience = "versus-api",
            Issuer = "versus"
        };
        var jwtConfigurationOptions = Options.Create(jwtConfiguration);
        _tokenService = new JwtTokenService(jwtConfigurationOptions);
    }

    [Fact]
    public void GenerateAccessTokenSuccess()
    {
        string token = _tokenService.GenerateAccessToken(_user);
        token.Should().NotBeNull();
        token.Should().NotBe(string.Empty);
    }

    [Fact]
    public void GenerateRefreshTokenSuccess()
    {
        string token = _tokenService.GenerateAccessToken(_user);
        token.Should().NotBeNull();
        token.Should().NotBe(string.Empty);
    }

    [Fact]
    public void ReadTokenSuccess()
    {
        string token = _tokenService.GenerateAccessToken(_user);
        var claimsPrincipal = _tokenService.ReadToken(token);
        string id = claimsPrincipal.Claims
            .Where(x => x.Type == ClaimTypes.NameIdentifier)
            .Select(x => x.Value)
            .First();
        id.Should().Be(_user.Id.ToString());
    }
}