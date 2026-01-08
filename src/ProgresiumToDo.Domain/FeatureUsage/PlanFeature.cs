using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Domain.FeatureUsage;

public sealed class PlanFeature
{
    public Guid PlanId { get; private set; }
    
    public Guid FeatureId { get; private set; }
    
    public int? DailyLimit { get; private set; }
    
    public int? MonthlyLimit { get; private set; }
    
    public Plan Plan { get; private set; }
    
    public Feature Feature { get; private set; }
    
    private PlanFeature() {}
    
    private PlanFeature(Guid planId, Guid featureId, int dailyLimit, int monthlyLimit)
    {
        PlanId = planId;
        FeatureId = featureId;
        DailyLimit = dailyLimit;
        MonthlyLimit = monthlyLimit;
    }
    
    public static PlanFeature Create(Guid planId, Guid featureId, int dailyLimit, int monthlyLimit)
    {
        return new PlanFeature(planId, featureId, dailyLimit, monthlyLimit);
    }
}