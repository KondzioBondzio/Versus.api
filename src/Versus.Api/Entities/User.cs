using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

[Table("Users")]
public class User : IdentityUser<Guid>
{
    [InverseProperty(nameof(UserRelationship.User))]
    public virtual ICollection<UserRelationship> Relationships { get; set; } = new HashSet<UserRelationship>();

    public virtual ICollection<UserClaim> UserClaims { get; set; } = new HashSet<UserClaim>();
    public virtual ICollection<UserLogin> UserLogins { get; set; } = new HashSet<UserLogin>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    public virtual ICollection<UserToken> UserTokens { get; set; } = new HashSet<UserToken>();

    #region Domain methods

    public bool IsFriendsWith(User user)
    {
        return Relationships.Any(x =>
            x is { Type: UserRelationshipType.Friend, Status: UserRelationshipStatus.Accepted }
            && (x.UserId == user.Id || x.RelatedUserId == user.Id));
    }

    public bool HasBlocked(User user)
    {
        return Relationships.Any(x => x is { Type: UserRelationshipType.Block, Status: UserRelationshipStatus.Accepted }
                                      && x.UserId == user.Id);
    }

    public bool IsBlockedBy(User user)
    {
        return Relationships.Any(x => x is { Type: UserRelationshipType.Block, Status: UserRelationshipStatus.Accepted }
                                      && x.RelatedUserId == user.Id);
    }

    public bool HasPendingRelationshipsWith(User user, UserRelationshipType type)
    {
        return Relationships.Any(x => x.Status == UserRelationshipStatus.Pending
                                      && x.Type == type
                                      && (x.UserId == user.Id || x.RelatedUserId == user.Id));
    }

    #endregion
}