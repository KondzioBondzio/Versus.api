using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

public class RoleClaim : IdentityRoleClaim<Guid>, IAuditableEntity
{
    public virtual Role Role { get; set; } = null!;
    
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}