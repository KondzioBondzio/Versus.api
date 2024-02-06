using System.ComponentModel.DataAnnotations.Schema;

namespace Versus.Domain.Entities;

[Table("UserRoles")]
public class UserRole
{
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;

    [ForeignKey(nameof(Role))]
    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;
}
