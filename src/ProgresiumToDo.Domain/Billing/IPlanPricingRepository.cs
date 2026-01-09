namespace ProgresiumToDo.Domain.Billing;

public interface IPlanPricingRepository
{
    Task<PlanPricing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}