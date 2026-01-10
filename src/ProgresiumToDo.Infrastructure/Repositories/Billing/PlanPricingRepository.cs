using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class PlanPricingRepository : Repository<PlanPricing>, IPlanPricingRepository
{
    public PlanPricingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}