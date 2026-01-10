using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface ISubscriptionRepository
{
    void Add(Subscription subscription);
    
    Task<Subscription> GetActiveSubscriptionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Subscription> GetActiveSubscriptionByUserIdWithPlanIncludedAsync(Guid userId,
        CancellationToken cancellationToken = default);
}