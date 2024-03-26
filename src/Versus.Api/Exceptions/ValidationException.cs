using Versus.Api.Validation;

namespace Versus.Api.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message, IEnumerable<ValidationError> validationErrors) : base(message)
    {
        ValidationErrors = validationErrors.ToList();
    }

    public IReadOnlyCollection<ValidationError> ValidationErrors { get; }
}