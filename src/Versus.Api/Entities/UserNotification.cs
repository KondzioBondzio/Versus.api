namespace Versus.Api.Entities;

public class UserNotification : EntityBase
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public Guid NotificationId { get; set; }
    public virtual Notification Notification { get; set; } = null!;

    public bool IsRead { get; set; }
}