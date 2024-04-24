using FluentValidation;

namespace Versus.Api.Filters;

public class RequestValidationFilter<TArguments, TRequest>(IValidator<TRequest>? validator = null) : IEndpointFilter
    where TArguments : class
    where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (validator is null)
        {
            return await next(context);
        }

        var args = context.Arguments.OfType<TArguments>().First();
        var request = args!.GetType()
            .GetProperties()
            .First(x => x.PropertyType == typeof(TRequest))
            .GetValue(args) as TRequest;
        var validationResult = await validator.ValidateAsync(request!, context.HttpContext.RequestAborted);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}