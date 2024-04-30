using Microsoft.AspNetCore.Identity;
using Versus.Api.Entities;

namespace Versus.Api.Validation.Validators;

public class IdentityUserValidator : IUserValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        return Task.FromResult(IdentityResult.Success);
    }
}