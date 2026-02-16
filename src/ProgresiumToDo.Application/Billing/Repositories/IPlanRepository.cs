using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface IPlanRepository
{
    Task<Plan?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default);

    Task<Plan?> GetByNameWithPricingsIncludedAsync(PlanType name, bool trackChanges = false, CancellationToken cancellationToken = default);
    
    Task<Plan?> GetByIdWithPricingsAndFeaturesIncludedAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default);
    
    Task<List<Plan>> GetAllWithPricingsAndFeaturesIncludedAsync(bool trackChanges = false, CancellationToken cancellationToken = default);
}