using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class PlanPricingRepository : Repository<PlanPricing>, IPlanPricingRepository
{
    public PlanPricingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PlanPricing?> GetByIdAsync(Guid id, bool includePlan = false, bool includeRegion = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PlanPricing> query = DbContext.PlanPricings;
        
        if (includePlan)
            query = query.Include(pp => pp.Plan);
        
        if (includeRegion)
            query = query.Include(pp => pp.Region);
        
        return await query.FirstOrDefaultAsync(pp => pp.Id == id, cancellationToken);
    }
}