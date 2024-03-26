using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

[Table("RoleClaims")]
public class RoleClaim : IdentityRoleClaim<Guid>
{
    public virtual Role Role { get; set; } = null!;
}