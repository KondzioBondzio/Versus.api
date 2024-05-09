namespace Versus.Api.Entities;

public class FeatureFilterParameter
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public Guid FeatureFilterId { get; set; }
    public virtual FeatureFilter FeatureFilter { get; set; } = null!;
}