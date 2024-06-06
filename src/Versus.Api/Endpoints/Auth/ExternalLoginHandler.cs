using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Versus.Api.Endpoints.Auth;

public class ExternalLoginHandler : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder) => builder
        .MapGet("/login/{scheme}", Handle)
        .AllowAnonymous();

    public static ChallengeHttpResult Handle(
        string scheme,
        CancellationToken cancellationToken)
    {
        // TODO: use LinkGenerator to generate callback url
        string redirectUrl = $"api/auth/login/{scheme}/callback";
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };

        return scheme switch
        {
            GoogleDefaults.AuthenticationScheme => TypedResults.Challenge(properties, [scheme]),
            _ => throw new NotSupportedException()
        };
    }
}