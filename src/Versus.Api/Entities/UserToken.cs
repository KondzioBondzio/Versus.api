using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

public class UserToken : IdentityUserToken<Guid>
{
    public virtual User User { get; set; } = null!;
}