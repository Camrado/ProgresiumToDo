using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface IPlanPricingRepository
{
    Task<PlanPricing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PlanPricing?> GetByIdAsync(Guid id, bool includePlan = false, bool includeRegion = false,
        CancellationToken cancellationToken = default);
}