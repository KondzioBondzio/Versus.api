namespace Versus.Shared.Auth;

public record LoginRequest
{
    public required string Login { get; init; } = string.Empty;
    public required string Password { get; init; } = string.Empty;
    public string TwoFactorCode { get; init; } = string.Empty;
    public string TwoFactorRecoveryCode { get; init; } = string.Empty;
}