using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Billing;

public interface ISubscriptionService
{
    Task<Result> SubscribeUserToFreePlanAsync(Guid userId, CancellationToken cancellationToken = default);
    
    Task<Result> SubscribeUserToPlanAsync(Guid userId, Guid planPricingId,
        CancellationToken cancellationToken = default);
}