namespace Versus.Shared.Auth;

public class RefreshTokenRequest
{
    public required string Token { get; set; } = string.Empty;
}