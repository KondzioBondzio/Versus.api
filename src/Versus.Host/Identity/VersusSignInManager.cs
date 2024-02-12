using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Versus.Core.Identity;
using Versus.Domain.Entities;

namespace Versus.Host.Identity;

public class VersusSignInManager : ISignInManager<User>
{
    private readonly SignInManager<User> _signInManager;

    public VersusSignInManager(SignInManager<User> signInManager) => _signInManager = signInManager;

    public string AuthenticationScheme
    {
        get => _signInManager.AuthenticationScheme;
        set => _signInManager.AuthenticationScheme = value;
    }

    public Task<User?> ValidateSecurityStampAsync(ClaimsPrincipal? principal) =>
        _signInManager.ValidateSecurityStampAsync(principal);

    public Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent,
        bool lockoutOnFailure) =>
        _signInManager.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

    public Task<SignInResult> TwoFactorAuthenticatorSignInAsync(string code, bool isPersistent, bool rememberClient) =>
        _signInManager.TwoFactorAuthenticatorSignInAsync(code, isPersistent, rememberClient);

    public Task<SignInResult> TwoFactorRecoveryCodeSignInAsync(string recoveryCode) =>
        _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

    public Task<ClaimsPrincipal> CreateUserPrincipalAsync(User user) => _signInManager.CreateUserPrincipalAsync(user);
}
