namespace Versus.Shared.Auth;

public record RegisterRequest
{
    public required string Login { get; init; } = string.Empty;
    public required string Password { get; init; } = string.Empty;
    public required string Email { get; init; } = string.Empty;
}