using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Domain.Billing;

public sealed class Plan : BaseEntity
{
    public PlanType Name { get; private set; }
    
    public string? Description { get; private set; }
    
    public ICollection<PlanPricing> PlanPricings { get; private set; } = new List<PlanPricing>();
    
    public ICollection<PlanFeature> PlanFeatures { get; private set; } = new List<PlanFeature>();
    
    private Plan(PlanType name, string? description)
    {
        Name = name;
        Description = description;
    }
}