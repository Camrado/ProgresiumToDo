using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface IPlanFeatureRepository
{
    Task<PlanFeature?> GetByFeatureNameAsync(Guid planId, FeatureName featureName, bool trackChanges = false, CancellationToken cancellationToken = default);
    
    Task<List<PlanFeature>> GetByPlanIdAsync(Guid planId, bool includeFeature = false, bool trackChanges = false, CancellationToken cancellationToken = default);
}