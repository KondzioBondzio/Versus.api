using Versus.Api.Entities;

namespace Versus.Api.Services.Auth;

public interface IAuthService
{
    Task<bool> ValidateCredentialsAsync(User user, string password);
}
