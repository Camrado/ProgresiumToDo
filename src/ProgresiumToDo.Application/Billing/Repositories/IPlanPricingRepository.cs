using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface IPlanPricingRepository
{
    Task<PlanPricing?> GetByIdAsync(Guid id, bool includePlan = false, bool includeRegion = false,
        bool trackChanges = false, CancellationToken cancellationToken = default);
}