using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

[Table("Users")]
public class User : IdentityUser<Guid>
{
    public virtual ICollection<UserClaim> UserClaims { get; set; } = new HashSet<UserClaim>();
    public virtual ICollection<UserLogin> UserLogins { get; set; } = new HashSet<UserLogin>();
    public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    public virtual ICollection<UserToken> UserTokens { get; set; } = new HashSet<UserToken>();
}
