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

    public Task CreateAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        return _userManager.CreateAsync(user, password);
    }
}
