namespace Versus.Api.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public byte[]? Image { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new HashSet<Room>();
}