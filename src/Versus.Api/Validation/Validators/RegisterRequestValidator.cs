using FluentValidation;
using Versus.Shared.Auth;

namespace Versus.Api.Validation.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .MinimumLength(4);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(4);
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}