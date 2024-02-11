using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Domain.Entities;

[Table("UserClaims")]
public class UserClaim : IdentityUserClaim<Guid>
{
    public virtual User User { get; set; } = null!;
}
