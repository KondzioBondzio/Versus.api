﻿namespace Versus.Api.Entities;

public class Feature : EntityBase
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<FeatureFilter> Filters { get; set; } = new HashSet<FeatureFilter>();
}