using System.ComponentModel.DataAnnotations.Schema;

namespace Versus.Api.Entities;

[Table("Permissions")]
public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;

    public virtual ICollection<RolePermission> PermissionRoles { get; set; } = new HashSet<RolePermission>();
}