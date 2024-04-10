namespace Versus.Api.Entities;

public class Permission
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<RolePermission> PermissionRoles { get; set; } = new HashSet<RolePermission>();
}