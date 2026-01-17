using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface IPlanFeatureRepository
{
    Task<PlanFeature?> GetByFeatureNameAsync(Guid planId, FeatureName featureName, CancellationToken cancellationToken = default);
}