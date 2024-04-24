using Versus.Api.Filters;

namespace Versus.Api.Extensions;

public static class RouteHandlerBuilderValidationExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TArgument, TRequest>(this RouteHandlerBuilder builder)
        where TArgument : class
        where TRequest : class
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TArgument, TRequest>>()
            .ProducesValidationProblem();
    }
}