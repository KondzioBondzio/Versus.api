namespace Versus.Api.Entities;

public interface IAuditableEntity
{
    Guid? CreatedBy { get; set; }
    DateTime CreatedDate { get; set; }

    Guid? UpdatedBy { get; set; }
    DateTime? UpdatedDate { get; set; }
}