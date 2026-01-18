namespace ProgresiumToDo.Application.Users.Queries.GetEntitlementStatus;

public sealed record GetEntitlementStatusQueryResponse(
    Guid SubscriptionId,
    string PlanName,
    string PlanDescription,
    DateOnly CycleStart,
    DateOnly CycleRenewsAt,
    List<FeatureEntitlementDto> Features
);

public sealed record FeatureEntitlementDto(
    Guid FeatureId,
    string FeatureName,
    bool IsUnlimited,
    QuotaStatus Daily,
    QuotaStatus Monthly,
    QuotaStatus Absolute
);

public sealed record QuotaStatus(
    int? Used,
    int? Limit)
{
    public int? Remaining => Limit.HasValue ? Limit - (Used ?? 0) : null;
    public bool? IsLimitReached => Limit.HasValue ? (Used ?? 0) >= Limit : null;
}