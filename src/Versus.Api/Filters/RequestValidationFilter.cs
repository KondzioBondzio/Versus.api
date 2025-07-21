using FluentValidation;

namespace Versus.Api.Filters;

public class RequestValidationFilter<TRequest> : IEndpointFilter
    where TRequest : class
{
    private readonly IValidator<TRequest>? _validator;
    private readonly Func<EndpointFilterInvocationContext, IDictionary<string, object?>>? _contextDataProvider;

    public RequestValidationFilter(IValidator<TRequest>? validator = null,
        Func<EndpointFilterInvocationContext, IDictionary<string, object?>>? contextDataProvider = null)
    {
        _validator = validator;
        _contextDataProvider = contextDataProvider;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_validator is null)
        {
            return await next(context);
        }

        var request = context.Arguments.OfType<TRequest>().First();
        var validationContext = CreateValidationContext(request, context);
        var validationResult = await _validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }

    private ValidationContext<TRequest> CreateValidationContext(TRequest request,
        EndpointFilterInvocationContext context)
    {
        var validationContext = new ValidationContext<TRequest>(request);
        if (_contextDataProvider != null)
        {
            var contextData = _contextDataProvider!(context);
            foreach ((string key, object? value) in contextData)
            {
                validationContext.RootContextData[key] = value;
            }
        }

        return validationContext;
    }
}