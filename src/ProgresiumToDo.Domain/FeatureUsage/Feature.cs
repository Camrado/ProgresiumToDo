namespace ProgresiumToDo.Domain.FeatureUsage;

public sealed class Feature
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    public FeatureName Name { get; private set; }
    
    public string? Description { get; private set; }
    
    public ICollection<FeatureUsage> FeatureUsages { get; private set; } = new List<FeatureUsage>();
    
    public ICollection<PlanFeature> PlanFeatures { get; private set; } = new List<PlanFeature>();
    
    private Feature(FeatureName name, string? description)
    {
        Name = name;
        Description = description;
    }
}