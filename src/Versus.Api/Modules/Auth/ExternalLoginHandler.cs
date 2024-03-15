using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Versus.Api.Modules.Auth;

public record ExternalLoginParameters
{
    public string Scheme { get; init; } = default!;
    public CancellationToken CancellationToken { get; init; } = default!;

    public void Deconstruct(out string scheme,
        out CancellationToken cancellationToken)
    {
        scheme = Scheme;
        cancellationToken = CancellationToken;
    }
}

public static class ExternalLoginHandler
{
    public static ChallengeHttpResult Handle
        ([AsParameters] ExternalLoginParameters parameters)
    {
        var (scheme, _) = parameters;

        // TODO: use LinkGenerator to generate callback url
        string redirectUrl = $"api/auth/login/{scheme}/callback";
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return scheme switch
        {
            GoogleDefaults.AuthenticationScheme => TypedResults.Challenge(properties, [scheme]),
            _ => throw new NotSupportedException()
        };
    }
}
