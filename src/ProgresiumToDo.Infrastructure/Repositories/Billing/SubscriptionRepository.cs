using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Subscription> GetActiveSubscriptionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Subscriptions
            .FirstAsync(s => s.UserId == userId && s.Status == SubscriptionStatus.Active, cancellationToken);
    }
}