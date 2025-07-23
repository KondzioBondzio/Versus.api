namespace Versus.Api.Entities;

public class FeatureFilter : EntityBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Guid FeatureId { get; set; }
    public virtual Feature Feature { get; set; } = null!;

    public virtual ICollection<FeatureFilterParameter> Parameters { get; set; } = new HashSet<FeatureFilterParameter>();
}