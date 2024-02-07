using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Versus.Domain.Entities;

namespace Versus.Core.Features.Auth;

public static class Login
{
    public record Request : IRequest<SignInResult>
    {
        public string Login { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string TwoFactorCode { get; init; } = string.Empty;
        public string TwoFactorRecoveryCode { get; init; } = string.Empty;
    }

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class RequestHandler : IRequestHandler<Request, SignInResult>
    {
        private readonly SignInManager<User> _signInManager;

        public RequestHandler(SignInManager<User> signInManager) => _signInManager = signInManager;

        public async Task<SignInResult> Handle(Request request, CancellationToken cancellationToken)
        {
            _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
            SignInResult result =
                await _signInManager.PasswordSignInAsync(request.Login, request.Password, false, false);
            if (result.RequiresTwoFactor)
            {
                if (!string.IsNullOrEmpty(request.TwoFactorCode))
                {
                    result = await _signInManager.TwoFactorAuthenticatorSignInAsync(request.TwoFactorCode,
                        false, false);
                }
                else if (!string.IsNullOrEmpty(request.TwoFactorRecoveryCode))
                {
                    result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(request.TwoFactorRecoveryCode);
                }
            }

            return result;
        }
    }
}
