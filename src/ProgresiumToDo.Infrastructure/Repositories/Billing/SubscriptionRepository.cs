using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Subscription?> GetActiveSubscriptionByUserIdAsync(Guid userId, bool includePlanPricing = false, bool includeRegion = false, 
        bool includePlan = false, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Subscription> query = DbContext.Subscriptions;

        if (!trackChanges)
            query = query.AsNoTracking();
        
        if (includePlan)
        {
            query = query.Include(s => s.PlanPricing).ThenInclude(pp => pp.Plan);
        }
        if (includeRegion)
        {
            query = query.Include(s => s.PlanPricing).ThenInclude(pp => pp.Region);
        }
        
        if (includePlanPricing && !includePlan && !includeRegion)
        {
            query = query
                .Include(s => s.PlanPricing);
        }
        
        return await query
            .SingleOrDefaultAsync(s => s.UserId == userId && s.Status == SubscriptionStatus.Active, cancellationToken);
    }
    
    public async Task<List<Subscription>> GetPaidSubscriptionsByUserIdAsync(Guid userId, bool includePlan = false,
        bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Subscription> query = DbContext.Subscriptions;

        if (!trackChanges)
            query = query.AsNoTracking();

        if (includePlan)
        {
            query = query
                .Include(s => s.PlanPricing).ThenInclude(pp => pp.Plan)
                .Include(s => s.PlanPricing).ThenInclude(pp => pp.Region);
        }
        
        return await query
            .Where(s => s.UserId == userId && s.PlanPricing.Plan.Name != PlanType.Free)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync(cancellationToken);
    }
}