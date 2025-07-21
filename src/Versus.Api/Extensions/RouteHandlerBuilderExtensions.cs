using FluentValidation;
using Versus.Api.Filters;

namespace Versus.Api.Extensions;

public static class RouteHandlerBuilderValidationExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
        where TRequest : class
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }

    public static RouteHandlerBuilder WithRequestValidation<TRequest>(
        this RouteHandlerBuilder builder,
        Func<EndpointFilterInvocationContext, IDictionary<string, object?>> contextDataProvider)
        where TRequest : class
    {
        return builder
            .AddEndpointFilterFactory((filterFactoryContext, next) =>
            {
                using var scope = filterFactoryContext.ApplicationServices.CreateScope();
                var validator = scope.ServiceProvider.GetService<IValidator<TRequest>>();
                var filter = new RequestValidationFilter<TRequest>(validator, contextDataProvider);
                return context => filter.InvokeAsync(context, next);
            })
            .ProducesValidationProblem();
    }
}