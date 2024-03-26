using System.ComponentModel.DataAnnotations.Schema;

namespace Versus.Api.Entities;

[Table("UserRelationships")]
public class UserRelationship
{
    public Guid Id { get; set; }

    [ForeignKey(nameof(Permission))]
    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;

    [ForeignKey(nameof(Permission))]
    public Guid RelatedUserId { get; set; }

    public virtual User RelatedUser { get; set; } = null!;

    public UserRelationshipType Type { get; set; }
    public UserRelationshipStatus Status { get; set; }
    public DateTime Timestamp { get; set; }
}