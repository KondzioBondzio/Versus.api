﻿namespace Versus.Api.Entities;

public class Room : EntityBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Password { get; set; }
    public DateTime PlayDate { get; set; } = DateTime.UtcNow;
    public RoomStatus Status { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Guid HostId { get; set; }
    public User Host { get; set; } = null!;

    public int TeamPlayerLimit { get; set; } = 1;

    public virtual ICollection<Team> Teams { get; set; } = new HashSet<Team>();
    public virtual ICollection<RoomUser> Users { get; set; } = new HashSet<RoomUser>();
    public virtual ICollection<RoomChatMessage> ChatMessages { get; set; } = new HashSet<RoomChatMessage>();
}