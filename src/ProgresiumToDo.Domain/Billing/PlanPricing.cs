using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Billing;

public sealed class PlanPricing : BaseEntity
{
    public decimal Price { get; private set; }
    
    public BillingPeriod BillingPeriod { get; private set; }
    
    public Guid PlanId { get; private set; }
    
    public Guid RegionId { get; private set; }
    
    public Plan Plan { get; private set; }
    
    public Region Region { get; private set; }
    
    private PlanPricing(decimal price, BillingPeriod billingPeriod, Guid planId, Guid regionId)
    {
        Price = price;
        BillingPeriod = billingPeriod;
        PlanId = planId;
        RegionId = regionId;
    }
    
    public static PlanPricing Create(decimal price, BillingPeriod billingPeriod, Guid planId, Guid regionId)
    {
        return new PlanPricing(price, billingPeriod, planId, regionId);
    }
}