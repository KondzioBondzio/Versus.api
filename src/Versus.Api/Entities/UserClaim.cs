using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

public class UserClaim : IdentityUserClaim<Guid>
{
    public virtual User User { get; set; } = null!;
}