using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Application.Abstractions.Auth.Entitlement;

public interface IEntitlementService
{
    Task<Result> TryIncrementUsageAsync(Guid userId, FeatureName featureName,
        CancellationToken cancellationToken = default);

    Task<Result<UserEntitlementSummary>> GetUserEntitlementsAsync(Guid userId, CancellationToken cancellationToken);
}