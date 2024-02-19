using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

[Table("Roles")]
public class Role : IdentityRole<Guid>
{
    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new HashSet<RoleClaim>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
    public virtual ICollection<UserRole> RoleUsers { get; set; } = new HashSet<UserRole>();
}
