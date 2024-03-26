namespace Versus.Shared.Auth;

public record RefreshTokenResponse
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}