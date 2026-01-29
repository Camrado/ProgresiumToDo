using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Billing.Queries.GetAllPlans;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Domain.Billing;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Application.Users.Queries.GetCurrentUser;

public sealed record GetCurrentUserQueryResponse(
    string Message,
    CurrentUserDto User);

public sealed record CurrentUserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsEmailVerified,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    SubscriptionDetailsDto? Subscription)
{
    public static CurrentUserDto FromDomain(User user, SubscriptionDetailsDto? subscription)
    {
        return new CurrentUserDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsEmailVerified,
            user.CreatedAt,
            user.UpdatedAt,
            subscription);
    }
}

public sealed record SubscriptionDetailsDto(
    Guid Id,
    string Status,
    DateTime StartDate,
    DateTime EndDate,
    bool IsAutoRenew,
    PlanDetailsDto Plan,
    List<FeatureEntitlementDto> Features)
{
    public static SubscriptionDetailsDto FromDomain(Subscription subscription, UserEntitlementSummary entitlements)
    {
        var featureEntitlements = entitlements.Features
            .Select(fs => FeatureEntitlementDto.FromDomain(fs.Definition, fs.Usage))
            .ToList();
        
        return new SubscriptionDetailsDto(
            subscription.Id,
            subscription.Status.ToString(),
            subscription.StartDate,
            subscription.EndDate,
            subscription.IsAutoRenew,
            PlanDetailsDto.FromDomain(subscription.PlanPricing),
            featureEntitlements);
    }
}

public sealed record PlanDetailsDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string BillingPeriod,
    RegionDto Region)
{
    public static PlanDetailsDto FromDomain(PlanPricing planPricing)
    {
        return new PlanDetailsDto(
            planPricing.Plan.Id,
            planPricing.Plan.Name.ToString(),
            planPricing.Plan.Description,
            planPricing.Price,
            planPricing.BillingPeriod.ToString(),
            new RegionDto(planPricing.Region.Code, planPricing.Region.Currency));
    }
}


public sealed record FeatureEntitlementDto(
    Guid FeatureId,
    string FeatureName,
    bool IsUnlimited,
    QuotaStatus Daily,
    QuotaStatus Monthly,
    QuotaStatus Absolute)
{
    public static FeatureEntitlementDto FromDomain(PlanFeature planFeature, FeatureUsageStats stats)
    {
        var isUnlimited = planFeature.DailyLimit is null && 
                          planFeature.MonthlyLimit is null && 
                          planFeature.AbsoluteLimit is null;
        
        return new FeatureEntitlementDto(
            planFeature.FeatureId,
            planFeature.Feature.Name.ToString(),
            isUnlimited,
            new QuotaStatus(stats.DailyUsage, planFeature.DailyLimit),
            new QuotaStatus(stats.MonthlyUsage, planFeature.MonthlyLimit),
            new QuotaStatus(stats.AbsoluteUsage, planFeature.AbsoluteLimit));
    }
}

public sealed record QuotaStatus(
    int? Used,
    int? Limit)
{
    public int? Remaining => Limit.HasValue ? Limit - (Used ?? 0) : null;
    public bool? IsLimitReached => Limit.HasValue ? (Used ?? 0) >= Limit : null;
}
