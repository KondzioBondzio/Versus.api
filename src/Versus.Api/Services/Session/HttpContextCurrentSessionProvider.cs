using System.Security.Claims;
using Versus.Api.Data;

namespace Versus.Api.Services.Session;

public class HttpContextCurrentSessionProvider(IHttpContextAccessor httpContextAccessor) : ICurrentSessionProvider
{
    public Guid? UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out var result) ? result : null;
        }
    }
}