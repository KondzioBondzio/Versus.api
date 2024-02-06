using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Domain.Entities;

[Table("Users")]
public class User : IdentityUser<int>
{
    public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
}
