using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

public class User : IdentityUser<Guid>, IAuditableEntity
{
    public new string UserName { get; set; } = null!;

    public byte[]? Image { get; set; }
    public string? FirstName { get; set; }
    public int YearOfBirth { get; set; }
    public UserGender Gender { get; set; }
    public string? LanguageCode { get; set; }
    public string? City { get; set; }

    public virtual ICollection<RoomChatMessage> ChatMessages { get; set; } = new HashSet<RoomChatMessage>();
    public virtual ICollection<Room> HostedRooms { get; set; } = new HashSet<Room>();
    public virtual ICollection<UserNotification> Notifications { get; set; } = new HashSet<UserNotification>();
    public virtual ICollection<RoomUser> RoomsInUse { get; set; } = new HashSet<RoomUser>();
    public virtual ICollection<UserRelationship> RelatedTo { get; set; } = new HashSet<UserRelationship>();
    public virtual ICollection<UserRelationship> RelatedUsers { get; set; } = new HashSet<UserRelationship>();
    public virtual ICollection<TeamUser> TeamsMember { get; set; } = new HashSet<TeamUser>();

    public virtual ICollection<UserClaim> UserClaims { get; set; } = new HashSet<UserClaim>();
    public virtual ICollection<UserLogin> UserLogins { get; set; } = new HashSet<UserLogin>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    public virtual ICollection<UserToken> UserTokens { get; set; } = new HashSet<UserToken>();

    public Guid? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}