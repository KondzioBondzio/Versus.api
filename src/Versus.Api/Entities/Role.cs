using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

public class Role : IdentityRole<Guid>, IAuditableEntity
{
    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new HashSet<RoleClaim>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new HashSet<RolePermission>();
    public virtual ICollection<UserRole> RoleUsers { get; set; } = new HashSet<UserRole>();

    public Guid? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}