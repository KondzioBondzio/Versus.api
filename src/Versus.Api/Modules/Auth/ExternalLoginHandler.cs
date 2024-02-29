using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Versus.Api.Modules.Auth;

public static class ExternalLoginHandler
{
    public static ChallengeHttpResult Handle
        (LinkGenerator linker, HttpContext context, string scheme, CancellationToken cancellationToken)
    {
        string redirectUrl = $"api/auth/login/{scheme}/callback";
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

        return scheme switch
        {
            GoogleDefaults.AuthenticationScheme => TypedResults.Challenge(properties, [scheme]),
            _ => throw new NotSupportedException()
        };
    }
}
