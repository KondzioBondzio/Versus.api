using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Versus.Domain.Entities;

namespace Versus.Core.Features.Auth;

public static class ResendConfirmationEmail
{
    public record Request(string Email) : IRequest;

    public class RequestHandler : IRequestHandler<Request>
    {
        private readonly UserManager<User> _userManager;

        public RequestHandler(UserManager<User> userManager) => _userManager = userManager;

        public async Task Handle(Request request, CancellationToken cancellationToken)
        {
            User? user = await _userManager.FindByEmailAsync(request.Email);

            if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
            {
                string code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = Convert.ToBase64String(Encoding.UTF8.GetBytes(code));

                //await emailSender.SendPasswordResetCodeAsync(user, resetRequest.Email, HtmlEncoder.Default.Encode(code));
            }
        }
    }
}
