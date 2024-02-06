using System.ComponentModel.DataAnnotations.Schema;

namespace Versus.Domain.Entities;

[Table("RolePermissions")]
public class RolePermission
{
    public int Id { get; set; }

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    [ForeignKey(nameof(Permission))]
    public int PermissionId { get; set; }

    public virtual Permission Permission { get; set; } = null!;
}
