using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Versus.Domain.Entities;

namespace Versus.Core.Features.Auth;

public static class Refresh
{
    public record Request : IRequest<ClaimsPrincipal?>
    {
        public Request(AuthenticationTicket? refreshTicket) => RefreshTicket = refreshTicket;

        public AuthenticationTicket? RefreshTicket { get; init; }
    }

    public class RequestHandler : IRequestHandler<Request, ClaimsPrincipal?>
    {
        private readonly SignInManager<User> _signInManager;

        public RequestHandler(SignInManager<User> signInManager) => _signInManager = signInManager;

        public async Task<ClaimsPrincipal?> Handle(Request request, CancellationToken cancellationToken)
        {
            if (request.RefreshTicket?.Properties?.ExpiresUtc is not { } expiresUtc
                || DateTime.UtcNow >= expiresUtc
                || await _signInManager.ValidateSecurityStampAsync(request.RefreshTicket.Principal) is not User user)
            {
                return null;
            }

            return await _signInManager.CreateUserPrincipalAsync(user);
        }
    }
}
