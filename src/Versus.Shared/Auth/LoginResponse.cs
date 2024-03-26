namespace Versus.Shared.Auth;

public record LoginResponse
{
    public string TokenType { get; } = "Bearer";
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}