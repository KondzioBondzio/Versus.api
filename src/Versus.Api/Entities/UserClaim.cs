using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

[Table("UserClaims")]
public class UserClaim : IdentityUserClaim<Guid>
{
    public virtual User User { get; set; } = null!;
}
