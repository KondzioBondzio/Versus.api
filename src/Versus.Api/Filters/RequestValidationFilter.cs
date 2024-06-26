﻿using FluentValidation;

namespace Versus.Api.Filters;

public class RequestValidationFilter<TRequest>(IValidator<TRequest>? validator = null) : IEndpointFilter
    where TRequest : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (validator is null)
        {
            return await next(context);
        }

        var request = context.Arguments.OfType<TRequest>().First();
        var validationResult = await validator.ValidateAsync(request!, context.HttpContext.RequestAborted);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}