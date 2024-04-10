namespace Versus.Api.Entities;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public Guid RoomId { get; set; }
    public virtual Room Room { get; set; } = null!;

    public virtual ICollection<TeamUser> Users { get; set; } = new HashSet<TeamUser>();
}