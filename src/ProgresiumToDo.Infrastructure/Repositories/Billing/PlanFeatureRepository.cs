using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Billing;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class PlanFeatureRepository : IPlanFeatureRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public PlanFeatureRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<PlanFeature?> GetByFeatureNameAsync(Guid planId, FeatureName featureName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.PlanFeatures
            .FirstOrDefaultAsync(pf => pf.PlanId == planId && pf.Feature.Name == featureName, cancellationToken);
    }

    public async Task<List<PlanFeature>> GetByPlanIdAsync(Guid planId, bool includeFeature = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.PlanFeatures
            .AsNoTracking()
            .Where(pf => pf.PlanId == planId);

        if (includeFeature)
        {
            query = query.Include(pf => pf.Feature);
        }
        
        return await query.ToListAsync(cancellationToken);
    }
}