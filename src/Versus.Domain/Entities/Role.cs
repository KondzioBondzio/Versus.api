using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Domain.Entities;

[Table("Roles")]
public class Role : IdentityRole<int>
{
    public virtual ICollection<UserRole> RoleUsers { get; set; } = new HashSet<UserRole>();

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
}
