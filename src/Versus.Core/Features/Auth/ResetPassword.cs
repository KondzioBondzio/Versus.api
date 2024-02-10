using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Versus.Domain.Entities;

namespace Versus.Core.Features.Auth;

public static class ResetPassword
{
    public record Request : IRequest<IdentityResult>
    {
        public string Email { get; init; } = string.Empty;
        public string ResetCode { get; init; } = string.Empty;
        public string NewPassword { get; init; } = string.Empty;
    }

    public class RequestHandler : IRequestHandler<Request, IdentityResult>
    {
        private readonly UserManager<User> _userManager;

        public RequestHandler(UserManager<User> userManager) => _userManager = userManager;

        public async Task<IdentityResult> Handle(Request request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                return IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            IdentityResult result;
            try
            {
                string code = Encoding.UTF8.GetString(Convert.FromBase64String(request.ResetCode));
                result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            return result;
        }
    }
}
