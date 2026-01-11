using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface ISubscriptionRepository
{
    void Add(Subscription subscription);

    Task<Subscription> GetActiveSubscriptionByUserIdAsync(Guid userId, bool includePlan = false,
        CancellationToken cancellationToken = default);

    Task<List<Subscription>> GetPaidSubscriptionsByUserIdAsync(Guid userId, bool includePlan = false,
        CancellationToken cancellationToken = default);
}