namespace Versus.Api.Entities;

public class Notification : EntityBase
{
    public Guid Id { get; set; }
    public int Source { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public virtual ICollection<UserNotification> NotificationUsers { get; set; } = new HashSet<UserNotification>();
}