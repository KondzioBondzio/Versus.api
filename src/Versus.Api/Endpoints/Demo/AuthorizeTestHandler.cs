using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Versus.Api.Endpoints.Demo;

public class AuthorizeTestHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapGet("/authorizeTest", Handle)
        .Produces<ContentHttpResult>()
        .Produces<UnauthorizedHttpResult>();

    public static IResult Handle(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
    {
        var identity = claimsPrincipal.Identity;
        if (!identity?.IsAuthenticated ?? false)
        {
            return TypedResults.Unauthorized();
        }

        string? id = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        string? email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine($"IsAuthenticated: {identity?.IsAuthenticated ?? false}");
        stringBuilder.AppendLine($"Id: {id}");
        stringBuilder.AppendLine($"Email: {email}");

        return TypedResults.Content(stringBuilder.ToString());
    }
}