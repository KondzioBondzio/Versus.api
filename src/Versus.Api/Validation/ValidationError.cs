namespace Versus.Api.Validation;

public class ValidationError(string property, string message)
{
    public string Property { get; } = property;
    public string Message { get; } = message;

    public override string ToString() => $"{Property} - {Message}";
}
