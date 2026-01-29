using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Domain.Billing;

public sealed class Subscription : BaseEntity
{
    public PlanType PlanName { get; private set; }
    
    public DateTime StartDate { get; private set; }
    
    public DateTime EndDate { get; private set; }
    
    public bool IsAutoRenew { get; private set; }
    
    public SubscriptionStatus Status { get; private set; }
    
    public Guid UserId { get; private set; }
    
    public Guid PlanPricingId { get; private set; }

    public User User { get; private set; }
    
    public PlanPricing PlanPricing { get; private set; }
    
    private Subscription(PlanType planName, DateTime startDate, DateTime endDate, bool isAutoRenew, 
        SubscriptionStatus status, Guid userId, Guid planPricingId)
    {
        PlanName = planName;
        StartDate = startDate;
        EndDate = endDate;
        IsAutoRenew = isAutoRenew;
        Status = status;
        UserId = userId;
        PlanPricingId = planPricingId;
    }

    public static Subscription Create(PlanType planName, DateTime startDate, DateTime endDate, bool isAutoRenew,
        Guid userId, Guid planPricingId)
    {
        return new Subscription(planName, startDate, endDate, isAutoRenew, SubscriptionStatus.Active, userId,
            planPricingId);
    }

    public void EndSubscription(DateTime endDate)
    {
        EndDate = endDate;
        Status = SubscriptionStatus.Cancelled;
    }
    
    public void FillPlanPricing(PlanPricing planPricing)
    {
        if (planPricing.Id != PlanPricingId)
            throw new InvalidOperationException("The provided PlanPricing does not match the Subscription's PlanPricingId.");
        
        PlanPricing = planPricing;
    }
}