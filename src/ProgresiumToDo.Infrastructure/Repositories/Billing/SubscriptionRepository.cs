using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}