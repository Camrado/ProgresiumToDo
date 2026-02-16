using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface ISubscriptionRepository
{
    void Add(Subscription subscription);

    Task<Subscription?> GetActiveSubscriptionByUserIdAsync(Guid userId, bool includePlanPricing = false, bool includeRegion = false, 
        bool includePlan = false, bool trackChanges = false, CancellationToken cancellationToken = default);

    Task<List<Subscription>> GetPaidSubscriptionsByUserIdAsync(Guid userId, bool includePlan = false,
        bool trackChanges = false, CancellationToken cancellationToken = default);
}