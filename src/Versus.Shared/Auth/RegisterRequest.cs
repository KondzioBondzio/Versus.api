using Versus.Shared.Enums;

namespace Versus.Shared.Auth;

public record RegisterRequest
{
    public required string Login { get; init; }
    public required string Password { get; init; }

    public required string UserName { get; init; }
    public byte[]? Image { get; init; }

    public string? FirstName { get; init; }
    public required int YearOfBirth { get; init; }
    public required Gender Gender { get; init; }

    public required string Language { get; init; }
    public required string City { get; init; }
}