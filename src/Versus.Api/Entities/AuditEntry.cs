namespace Versus.Api.Entities;

public class AuditEntry
{
    public Guid Id { get; set; }
    public string EntityName { get; set; } = null!;
    public string EntityId { get; set; } = null!;
    public AuditLogChangeType ChangeType { get; set; }
    public string ChangedValues { get; set; } = null!;
    public Guid? ChangedBy { get; set; }
    public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
}