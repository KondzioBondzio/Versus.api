namespace Versus.Api.Entities;

public class RolePermission : EntityBase
{
    public Guid Id { get; set; }

    public Guid RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
    
    public Guid PermissionId { get; set; }
    public virtual Permission Permission { get; set; } = null!;
}