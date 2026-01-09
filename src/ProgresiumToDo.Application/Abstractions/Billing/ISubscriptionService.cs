using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Abstractions.Billing;

public interface ISubscriptionService
{
    Task<Result> SubscribeUserToFreePlanAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<Result<Subscription>> SubscribeUserToPlanAsync(Guid userId, PlanPricing planPricing, bool isAutoRenew,
        CancellationToken cancellationToken = default);
}