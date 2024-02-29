using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Versus.Api.Configuration;
using Versus.Api.Entities;

namespace Versus.Api.Services.Auth;

public class JwtTokenService : ITokenService
{
    private readonly string _audience;
    private readonly string _issuer;
    private readonly SecurityKey _key;

    public JwtTokenService(IOptions<JwtTokenConfiguration> options)
    {
        _audience = options.Value.Audience;
        _issuer = options.Value.Issuer;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key));
    }

    public string GenerateAccessToken(User user)
    {
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GetClaimsIdentityFromUser(user),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddMinutes(15),
            Audience = _audience,
            Issuer = _issuer
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GetClaimsIdentityFromUser(user),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(24),
            Audience = _audience,
            Issuer = _issuer
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal ReadToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidIssuer = _issuer,
            ClockSkew = TimeSpan.Zero
        };
        return tokenHandler.ValidateToken(token, validationParameters, out _);
    }

    public bool IsTokenValid(string token)
    {
        try
        {
            _ = ReadToken(token);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static ClaimsIdentity GetClaimsIdentityFromUser(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        return new ClaimsIdentity(claims);
    }
}
