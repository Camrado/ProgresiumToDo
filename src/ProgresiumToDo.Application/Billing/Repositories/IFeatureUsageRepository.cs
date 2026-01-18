using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Application.Billing.Repositories;

public interface IFeatureUsageRepository
{
    Task<FeatureUsageStats> GetUsedFeatureQuotaAsync(Guid userId, FeatureName featureName, DateOnly today,
        DateOnly firstDayOfSubscriptionMonth, CancellationToken cancellationToken = default);

    Task IncrementFeatureUsageAsync(Guid userId, FeatureName featureName, int incrementBy, DateOnly usageDate,
        CancellationToken cancellationToken = default);
}