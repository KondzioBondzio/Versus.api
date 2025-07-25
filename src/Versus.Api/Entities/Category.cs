﻿namespace Versus.Api.Entities;

public class Category : EntityBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public byte[]? Image { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new HashSet<Room>();
}