using System.Text.RegularExpressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Versus.Api.Validation.Validators;

public class UserPasswordValidator<T> : PropertyValidator<T, string>
{
    private static readonly Regex _regex = CreateRegEx();
    private const string Expression = @".{4,}";

    public override string Name => "PasswordValidator";

    public override bool IsValid(ValidationContext<T> context, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!_regex.IsMatch(value))
        {
            return false;
        }

        return true;
    }

    protected override string GetDefaultMessageTemplate(string errorCode)
        => "Password must have at least 4 characters";

    private static Regex CreateRegEx()
    {
        const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
        return new Regex(Expression, options, TimeSpan.FromSeconds(2.0));
    }
}