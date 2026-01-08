using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class PlanRepository : Repository<Plan>, IPlanRepository
{
    public PlanRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<Plan?> GeyByNameWithPricingsIncludedAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbContext.Plans
            .Include(p => p.PlanPricings)
                .ThenInclude(pp => pp.Region)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<Plan?> GetByIdWithPricingsAndFeaturesIncludedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Plans
            .Include(p => p.PlanPricings)
            .ThenInclude(pp => pp.Region)
            .Include(p => p.PlanFeatures)
            .ThenInclude(pf => pf.Feature)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Plan>> GetAllWithPricingsAndFeaturesIncludedAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Plans
            .Include(p => p.PlanPricings)
                .ThenInclude(pp => pp.Region)
            .Include(p => p.PlanFeatures)
                .ThenInclude(pf => pf.Feature)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}