using Microsoft.AspNetCore.Identity;
using Versus.Api.Entities;

namespace Versus.Api.Validation.Validators;

public class IdentityUserValidator : IUserValidator<User>
{
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        var exists = manager.Users.Any(x => x.Email!.ToLower() == user.Email!.ToLower());
        if (exists)
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = nameof(User.Email), Description = "This email is already associated with an account"
            }));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}