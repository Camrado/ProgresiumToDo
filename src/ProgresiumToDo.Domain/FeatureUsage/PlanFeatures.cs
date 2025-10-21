using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Domain.FeatureUsage;

public sealed class PlanFeatures
{
    public Guid PlanId { get; private set; }
    
    public Guid FeatureId { get; private set; }
    
    public int DailyLimit { get; private set; }
    
    public int MonthlyLimit { get; private set; }
    
    public Plan Plan { get; private set; }
    
    public Feature Feature { get; private set; }
    
    private PlanFeatures(Guid planId, Guid featureId, int dailyLimit, int monthlyLimit)
    {
        PlanId = planId;
        FeatureId = featureId;
        DailyLimit = dailyLimit;
        MonthlyLimit = monthlyLimit;
    }
    
    public static PlanFeatures Create(Guid planId, Guid featureId, int dailyLimit, int monthlyLimit)
    {
        return new PlanFeatures(planId, featureId, dailyLimit, monthlyLimit);
    }
}