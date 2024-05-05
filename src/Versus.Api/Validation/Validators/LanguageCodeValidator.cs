using System.Globalization;
using FluentValidation;
using FluentValidation.Validators;

namespace Versus.Api.Validation.Validators;

public class LanguageCodeValidator<T> : PropertyValidator<T, string>
{
    public override string Name => "LanguageCodeValidator";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        try
        {
            var cultureInfo = CultureInfo.CreateSpecificCulture(value);
            return cultureInfo.TwoLetterISOLanguageName == value;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }

    protected override string GetDefaultMessageTemplate(string errorCode) 
        => "Invalid language code";
}