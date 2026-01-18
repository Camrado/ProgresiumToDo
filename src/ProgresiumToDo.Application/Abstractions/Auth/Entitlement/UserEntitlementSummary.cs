using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Application.Abstractions.Auth.Entitlement;

public sealed record UserEntitlementSummary(
    Guid SubscriptionId,
    string PlanName,
    string? PlanDescription,
    DateOnly CycleStart,
    DateOnly CycleRenewsAt,
    List<FeatureStatus> Features
);

public sealed record FeatureStatus(
    PlanFeature Definition,
    FeatureUsageStats Usage
);