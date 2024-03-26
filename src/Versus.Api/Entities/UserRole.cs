using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

[Table("UserRoles")]
public class UserRole : IdentityUserRole<Guid>
{
    public virtual User User { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}