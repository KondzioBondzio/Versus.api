using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Domain.Entities;

[Table("UserLogins")]
public class UserLogin : IdentityUserLogin<Guid>
{
    // [ForeignKey(nameof(UserId))]
    // public virtual User User { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}
