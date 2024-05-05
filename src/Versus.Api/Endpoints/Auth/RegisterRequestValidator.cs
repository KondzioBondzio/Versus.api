using FluentValidation;
using Versus.Api.Extensions;
using Versus.Shared.Auth;

namespace Versus.Api.Endpoints.Auth;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private const int MinimumAgeRequirement = 13;

    public RegisterRequestValidator()
    {
        RuleFor(x => x.Login)
            .EmailAddress();
        RuleFor(x => x.Password)
            .UserPassword();

        RuleFor(x => x.UserName)
            .NotEmpty();

        RuleFor(x => x.YearOfBirth)
            .NotEmpty()
            .LessThan(DateTime.Today.Year - MinimumAgeRequirement);

        RuleFor(x => x.Language)
            .TwoLetterISOLanguageCode();
        RuleFor(x => x.City)
            .NotEmpty();
    }
}