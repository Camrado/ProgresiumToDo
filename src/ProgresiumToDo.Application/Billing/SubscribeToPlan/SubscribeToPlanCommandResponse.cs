namespace ProgresiumToDo.Application.Billing.SubscribeToPlan;

public sealed record SubscribeToPlanCommandResponse(
    string Message,
    SubscriptionDto Subscription);
    
public sealed record SubscriptionDto(
    Guid Id,
    Guid PlanPricingId,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    bool IsAutoRenew);