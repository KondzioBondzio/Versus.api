using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

public class RoleClaim : IdentityRoleClaim<Guid>
{
    public virtual Role Role { get; set; } = null!;
}