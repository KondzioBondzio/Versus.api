namespace Versus.Shared.Auth;

public record LoginRequest
{
    public required string Login { get; init; }
    public required string Password { get; init; }
    public string TwoFactorCode { get; init; } = string.Empty;
    public string TwoFactorRecoveryCode { get; init; } = string.Empty;
}