using Versus.Api.Entities;

namespace Versus.Api.Services.Auth;

public interface IUserService
{
    Task<User?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task CreateAsync(User user, string password, CancellationToken cancellationToken = default);
}