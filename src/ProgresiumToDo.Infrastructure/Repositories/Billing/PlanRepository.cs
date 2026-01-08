using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class PlanRepository : Repository<Plan>, IPlanRepository
{
    public PlanRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Plan>> GetAllAsync(CancellationToken cancellationToken = default)
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