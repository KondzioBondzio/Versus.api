using Microsoft.AspNetCore.Diagnostics;

namespace Versus.Api.Exceptions;

public class ApiExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ApiExceptionHandler> _logger;

    public ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        (int statusCode, string errorMessage) = exception switch
        {
            _ => (500, "Something went wrong")
        };
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsync(errorMessage, cancellationToken: cancellationToken);

        _logger.LogError(exception, exception.Message);

        return true;
    }
}