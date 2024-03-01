using Microsoft.AspNetCore.Identity;
using Versus.Api.Entities;
using Versus.Api.Validation;

namespace Versus.Api.Services.Auth;

public class EfUserService : IUserService
{
    private readonly UserManager<User> _userManager;

    public EfUserService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public Task<User?> FindByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return _userManager.FindByIdAsync(id);
    }

    public Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _userManager.FindByEmailAsync(email);
    }

    public async Task CreateAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            return;
        }

        ValidationState validationState = new();
        foreach (var error in result.Errors)
        {
            validationState.AddError(error.Code, error.Description);
        }

        validationState.EnsureValid();
    }
}
