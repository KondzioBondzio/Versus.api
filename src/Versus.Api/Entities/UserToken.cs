using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Versus.Api.Entities;

[Table("UserTokens")]
public class UserToken : IdentityUserToken<Guid>
{
    public virtual User User { get; set; } = null!;
}