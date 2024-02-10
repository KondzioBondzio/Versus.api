using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Domain.Entities;

[Table("RoleClaims")]
public class RoleClaim : IdentityRoleClaim<Guid>
{
    // [ForeignKey(nameof(RoleId))]
    // public virtual Role Role { get; set; } = null!;
}
