using System.Security.Claims;
using Microsoft.Extensions.Options;
using Versus.Api.Configuration;
using Versus.Api.Entities;
using Versus.Api.Services;
using Versus.Api.Services.Auth;

namespace Versus.Api.Tests.Auth;

public class TokenServiceTest
{
    private readonly JwtTokenService _tokenService;

    private readonly User _user = new()
    {
        Id = Guid.NewGuid(),
        UserName = "DemoUser",
        Email = "DemoUser@versus.vs"
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
        Assert.NotNull(token);
        Assert.NotEqual(string.Empty, token);
    }

    [Fact]
    public void GenerateRefreshTokenSuccess()
    {
        string token = _tokenService.GenerateAccessToken(_user);
        Assert.NotNull(token);
        Assert.NotEqual(string.Empty, token);
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
        Assert.Equal(_user.Id, Guid.Parse(id));
    }
}