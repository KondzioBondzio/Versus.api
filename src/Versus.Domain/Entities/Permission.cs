namespace Versus.Domain.Entities;

public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }

    public virtual ICollection<RolePermission> PermissionRoles { get; set; } = new HashSet<RolePermission>();
}
