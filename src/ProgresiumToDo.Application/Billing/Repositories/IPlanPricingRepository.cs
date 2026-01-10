using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface IPlanPricingRepository
{
    Task<PlanPricing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}