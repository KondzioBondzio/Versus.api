namespace Versus.Api.Entities;

public class RoomUser
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }
    public virtual Room Room { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}