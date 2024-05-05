using FluentValidation;
using Versus.Api.Extensions;
using Versus.Shared.Auth;

namespace Versus.Api.Endpoints.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .UserPassword();
    }
}