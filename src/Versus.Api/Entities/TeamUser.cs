namespace Versus.Api.Entities;

public class TeamUser
{
    public Guid Id { get; set; }

    public Guid TeamId { get; set; }
    public virtual Team Team { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}