using FluentValidation;
using FluentValidation.Validators;
using Versus.Api.Validation.Validators;

namespace Versus.Api.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> UserPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        var validator = (PropertyValidator<T, string>)new UserPasswordValidator<T>();
        return ruleBuilder.SetValidator(validator);
    }

    public static IRuleBuilderOptions<T, string> TwoLetterISOLanguageCode<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        var validator = (PropertyValidator<T, string>)new LanguageCodeValidator<T>();
        return ruleBuilder.SetValidator(validator);
    }
}