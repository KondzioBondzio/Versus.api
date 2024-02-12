using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Versus.Core.Identity;

public interface ISignInManager<TUser> where TUser : class
{
    public string AuthenticationScheme { get; set; }

    Task<TUser?> ValidateSecurityStampAsync(ClaimsPrincipal? principal);
    Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);
    Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient);
    Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode);
    Task<ClaimsPrincipal> CreateUserPrincipalAsync(TUser user);
}
