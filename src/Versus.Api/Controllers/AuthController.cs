using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Versus.Core.Features.Auth;

namespace Versus.Api.Controllers;

[AllowAnonymous]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IOptionsMonitor<BearerTokenOptions> _optionsMonitor;

    public AuthController(IMediator mediator, IOptionsMonitor<BearerTokenOptions> optionsMonitor)
    {
        _mediator = mediator;
        _optionsMonitor = optionsMonitor;
    }

    [HttpPost("register")]
    public async Task<Results<Ok, ValidationProblem>> Register
        ([FromBody] Register.Request request, CancellationToken cancellationToken = default)
    {
        IdentityResult result = await _mediator.Send(request, cancellationToken);
        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        return TypedResults.Ok();
    }

    [HttpPost("login")]
    public async Task<Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>> Login
        ([FromBody] Login.Request request, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(request, cancellationToken);
        if (!result.Succeeded)
        {
            return TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized);
        }

        return TypedResults.Empty;
    }

    [HttpPost("refresh")]
    public async Task<Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>>
        Refresh([FromBody] RefreshRequest request, CancellationToken cancellationToken = default)
    {
        string scheme = IdentityConstants.BearerScheme;
        ISecureDataFormat<AuthenticationTicket> tokenProtector = _optionsMonitor.Get(scheme).RefreshTokenProtector;
        AuthenticationTicket? refreshTicket = tokenProtector.Unprotect(request.RefreshToken);

        ClaimsPrincipal? result = await _mediator.Send(new Refresh.Request(refreshTicket), cancellationToken);
        if (result == null)
        {
            return TypedResults.Challenge();
        }

        return TypedResults.SignIn(result, authenticationScheme: IdentityConstants.BearerScheme);
    }

    [HttpGet("confirmEmail")]
    public async Task<Results<Ok, UnauthorizedHttpResult>> ConfirmEmail(
        [FromQuery] string id, [FromQuery] string code, CancellationToken cancellationToken = default)
    {
        IdentityResult result = await _mediator.Send(new ConfirmEmail.Request(id, code), cancellationToken);
        if (!result.Succeeded)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok();
    }

    [HttpPost("resendConfirmationEmail")]
    public async Task<IActionResult> ResendConfirmationEmail
        ([FromBody] ResendConfirmationEmail.Request request, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(request, cancellationToken);
        return Ok();
    }

    [HttpPost("forgotPassword")]
    public async Task<IActionResult> ForgotPassword
        (ForgotPassword.Request request, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(request, cancellationToken);
        return Ok();
    }

    [HttpPost("resetPassword")]
    public async Task<Results<Ok, ValidationProblem>> ResetPassword(ResetPassword.Request request,
        CancellationToken cancellationToken = default)
    {
        IdentityResult result = await _mediator.Send(request, cancellationToken);
        if (!result.Succeeded)
        {
            return CreateValidationProblem(result);
        }

        return TypedResults.Ok();
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        Dictionary<string, string[]> errorDictionary = new(1);
        foreach (IdentityError? error in result.Errors)
        {
            string[] newDescriptions;
            if (errorDictionary.TryGetValue(error.Code, out string[]? descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }
}
