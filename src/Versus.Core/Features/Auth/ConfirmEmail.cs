using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Versus.Domain.Entities;

namespace Versus.Core.Features.Auth;

public static class ConfirmEmail
{
    public record Request(string Id, string Code) : IRequest<IdentityResult>;

    public class RequestHandler : IRequestHandler<Request, IdentityResult>
    {
        private readonly UserManager<User> _userManager;

        public RequestHandler(UserManager<User> userManager) => _userManager = userManager;

        public async Task<IdentityResult> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await _userManager.FindByIdAsync(request.Id) is not { } user)
            {
                return IdentityResult.Failed(new IdentityError());
            }

            string code = request.Code;
            try
            {
                code = Encoding.UTF8.GetString(Convert.FromBase64String(code));
            }
            catch (FormatException)
            {
                return IdentityResult.Failed(new IdentityError());
            }

            return await _userManager.ConfirmEmailAsync(user, code);
        }
    }
}
