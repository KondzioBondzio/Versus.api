using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Versus.Domain.Entities;

namespace Versus.Core.Features.Auth;

public static class Register
{
    public record Request : IRequest<IdentityResult>
    {
        public string Login { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
    }

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Email).NotEmpty();
        }
    }

    public class RequestHandler : IRequestHandler<Request, IdentityResult>
    {
        private readonly UserManager<User> _userManager;

        public RequestHandler(UserManager<User> userManager) => _userManager = userManager;

        public async Task<IdentityResult> Handle(Request request, CancellationToken cancellationToken)
        {
            User user = new()
            {
                UserName = request.Login,
                Email = request.Email
            };

            IdentityResult identityResult = await _userManager.CreateAsync(user, request.Password);
            if (identityResult.Succeeded)
            {
                // send confirmation email
            }

            return identityResult;
        }
    }
}
