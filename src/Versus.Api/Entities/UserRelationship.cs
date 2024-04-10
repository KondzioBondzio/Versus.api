namespace Versus.Api.Entities;

public class UserRelationship
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public Guid RelatedUserId { get; set; }

    public virtual User RelatedUser { get; set; } = null!;

    public UserRelationshipType Type { get; set; }
    public UserRelationshipStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
}