namespace Versus.Shared.Relationships;

public record RelationshipDto
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required RelationshipDirection Direction { get; init; }
    public required int Status { get; init; }
    public required int Type { get; init; }

    public enum RelationshipDirection
    {
        Incoming,
        Outgoing
    }
}