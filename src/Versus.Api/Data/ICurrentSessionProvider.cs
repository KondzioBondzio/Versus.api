namespace Versus.Api.Data;

public interface ICurrentSessionProvider
{
    Guid? UserId { get; }
}