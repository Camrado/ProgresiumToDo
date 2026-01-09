namespace ProgresiumToDo.Domain.Billing;

public interface ISubscriptionRepository
{
    void Add(Subscription subscription);
    
    Task<Subscription> GetActiveSubscriptionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}