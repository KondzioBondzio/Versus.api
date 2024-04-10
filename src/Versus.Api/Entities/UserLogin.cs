using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

public class UserLogin : IdentityUserLogin<Guid>
{
    public virtual User User { get; set; } = null!;

    public DateTime Timestamp { get; set; }
}