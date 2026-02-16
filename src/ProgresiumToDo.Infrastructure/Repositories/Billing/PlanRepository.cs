using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class PlanRepository : Repository<Plan>, IPlanRepository
{
    public PlanRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<Plan?> GetByNameWithPricingsIncludedAsync(PlanType name, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Plan> query = DbContext.Plans
            .Include(p => p.PlanPricings)
                .ThenInclude(pp => pp.Region);

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(p => p.Name == name, cancellationToken);
    }

    public async Task<Plan?> GetByIdWithPricingsAndFeaturesIncludedAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Plan> query = DbContext.Plans
            .Include(p => p.PlanPricings)
            .ThenInclude(pp => pp.Region)
            .Include(p => p.PlanFeatures)
            .ThenInclude(pf => pf.Feature);

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Plan>> GetAllWithPricingsAndFeaturesIncludedAsync(bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Plan> query = DbContext.Plans
            .Include(p => p.PlanPricings)
                .ThenInclude(pp => pp.Region)
            .Include(p => p.PlanFeatures)
                .ThenInclude(pf => pf.Feature);

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.ToListAsync(cancellationToken);
    }
}