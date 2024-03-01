using Versus.Api.Exceptions;

namespace Versus.Api.Validation;

public class ValidationState
{
    private readonly List<ValidationError> _errors = [];

    public bool IsValid => _errors.Count == 0;

    public IReadOnlyCollection<ValidationError> Errors => _errors;

    public void AddError(string property, string message)
    {
        _errors.Add(new ValidationError(property, message));
    }

    public void EnsureValid()
    {
        if (!IsValid)
        {
            throw new ValidationException("Validation failed", _errors);
        }
    }
}
