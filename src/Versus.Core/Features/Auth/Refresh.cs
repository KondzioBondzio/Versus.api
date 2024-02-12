using System.Security.Claims;
using MediatR;
using Versus.Core.Identity;
using Versus.Domain.Entities;

namespace Versus.Core.Features.Auth;

public static class Refresh
{
    public record Request : IRequest<ClaimsPrincipal?>
    {
        public Request(ClaimsPrincipal? principal, DateTimeOffset? expiresUtc)
        {
            Principal = principal;
            ExpiresUtc = expiresUtc;
        }

        public ClaimsPrincipal? Principal { get; init; }
        public DateTimeOffset? ExpiresUtc { get; init; }
    }

    public class RequestHandler : IRequestHandler<Request, ClaimsPrincipal?>
    {
        private readonly ISignInManager<User> _signInManager;

        public RequestHandler(ISignInManager<User> signInManager) => _signInManager = signInManager;

        public async Task<ClaimsPrincipal?> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request.ExpiresUtc is not { } expiresUtc
                || DateTime.UtcNow >= expiresUtc
                || await _signInManager.ValidateSecurityStampAsync(request.Principal) is not User user)
            {
                return null;
            }

            return await _signInManager.CreateUserPrincipalAsync(user);
        }
    }
}
