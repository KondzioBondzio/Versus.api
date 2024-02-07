using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Versus.Domain.Entities;

namespace Versus.Core.Features.Auth;

public static class TwoFactorAuthentication
{
    public record Request : IRequest<TwoFactorResponse?>
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public bool? Enable { get; init; }
        public string? TwoFactorCode { get; init; }
        public bool ResetSharedKey { get; init; }
        public bool ResetRecoveryCodes { get; init; }
        public bool ForgetMachine { get; init; }
    }

    public class RequestHandler : IRequestHandler<Request, TwoFactorResponse?>
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public RequestHandler(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<TwoFactorResponse?> Handle(Request request, CancellationToken cancellationToken)
        {
            if (await _userManager.GetUserAsync(request.ClaimsPrincipal) is not { } user)
            {
                return null;
            }

            if (request.Enable == true)
            {
                if (request.ResetSharedKey)
                {
                    // "Resetting the 2fa shared key must disable 2fa until a 2fa token based on the new shared key is validated."
                    return null;
                }
                else if (string.IsNullOrEmpty(request.TwoFactorCode))
                {
                    // "No 2fa token was provided by the request. A valid 2fa token is required to enable 2fa."
                    return null;
                }
                else if (!await _userManager.VerifyTwoFactorTokenAsync(user,
                             _userManager.Options.Tokens.AuthenticatorTokenProvider, request.TwoFactorCode))
                {
                    // "The 2fa token provided by the request was invalid. A valid 2fa token is required to enable 2fa."
                    return null;
                }

                await _userManager.SetTwoFactorEnabledAsync(user, true);
            }
            else if (request.Enable == false || request.ResetSharedKey)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
            }

            if (request.ResetSharedKey)
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
            }

            string[]? recoveryCodes = null;
            if (request.ResetRecoveryCodes
                || (request.Enable == true && await _userManager.CountRecoveryCodesAsync(user) == 0))
            {
                IEnumerable<string>? recoveryCodesEnumerable =
                    await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                recoveryCodes = recoveryCodesEnumerable?.ToArray();
            }

            if (request.ForgetMachine)
            {
                await _signInManager.ForgetTwoFactorClientAsync();
            }

            string? key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                key = await _userManager.GetAuthenticatorKeyAsync(user);

                if (string.IsNullOrEmpty(key))
                {
                    throw new NotSupportedException("The user manager must produce an authenticator key after reset.");
                }
            }

            return new TwoFactorResponse
            {
                SharedKey = key,
                RecoveryCodes = recoveryCodes,
                RecoveryCodesLeft = recoveryCodes?.Length ?? await _userManager.CountRecoveryCodesAsync(user),
                IsTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user)
            };
        }
    }
}
