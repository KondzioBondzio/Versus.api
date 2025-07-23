namespace Versus.Api.Entities;

public class RoomChatMessage : EntityBase
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }
    public virtual Room Room { get; set; } = null!;

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string Content { get; set; } = string.Empty;
}