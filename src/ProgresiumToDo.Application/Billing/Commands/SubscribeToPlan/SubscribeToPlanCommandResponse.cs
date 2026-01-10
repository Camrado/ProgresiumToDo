using ProgresiumToDo.Application.Billing.Queries.GetAllPlans;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;

public sealed record SubscribeToPlanCommandResponse(
    string Message,
    SubscriptionDto Subscription);

public sealed record SubscriptionDto(
    Guid Id,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    bool IsAutoRenew,
    SubscriptionPlanDto Plan)
{
    public static SubscriptionDto FromDomain(Subscription sub)
    {
        return new SubscriptionDto(
            sub.Id,
            sub.Status.ToString(),
            sub.StartDate,
            sub.EndDate,
            sub.IsAutoRenew,
            new SubscriptionPlanDto(
                sub.PlanPricing.Plan.Id,
                sub.PlanPricing.Plan.Name,
                sub.PlanPricing.Plan.Description,
                sub.PlanPricing.Price,
                sub.PlanPricing.BillingPeriod.ToString(),
                new RegionDto(
                    sub.PlanPricing.Region.Code,
                    sub.PlanPricing.Region.Currency)));
    }
}
    
public sealed record SubscriptionPlanDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string BillingPeriod,
    RegionDto Region);