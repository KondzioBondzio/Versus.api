using Microsoft.AspNetCore.Identity;
using Versus.Api.Entities;

namespace Versus.Api.Services.Auth;

public class EfAuthService : IAuthService
{
    private readonly SignInManager<User> _signInManager;

    public EfAuthService(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<bool> ValidateCredentialsAsync(User user, string password)
    {
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        return result.Succeeded;
    }
}
