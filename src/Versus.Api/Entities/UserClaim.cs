using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

public class UserClaim : IdentityUserClaim<Guid>, IAuditableEntity
{
    public virtual User User { get; set; } = null!;

    public Guid? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}