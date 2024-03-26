using System.Security.Claims;
using Versus.Api.Entities;

namespace Versus.Api.Services.Auth;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(User user);
    ClaimsPrincipal ReadToken(string token);
    bool IsTokenValid(string token);
}