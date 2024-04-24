using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Versus.Api.Entities;

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

        var validationResult = new ValidationResult();
        foreach (var error in result.Errors)
        {
            validationResult.Errors.Add(new ValidationFailure(error.Code, error.Description));
        }

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
}