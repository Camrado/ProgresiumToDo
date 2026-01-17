using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Billing.Errors;
using ProgresiumToDo.Domain.FeatureUsage;
using ProgresiumToDo.Domain.FeatureUsage.Errors;

namespace ProgresiumToDo.Infrastructure.Auth.Entitlement;

internal sealed class EntitlementService : IEntitlementService
{
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IPlanFeatureRepository _planFeatureRepository;
    private readonly IFeatureUsageRepository _featureUsageRepository;
    
    public EntitlementService(
        ISubscriptionRepository subscriptionRepository,
        IPlanFeatureRepository planFeatureRepository,
        IFeatureUsageRepository featureUsageRepository)
    {
        _subscriptionRepository = subscriptionRepository;
        _planFeatureRepository = planFeatureRepository;
        _featureUsageRepository = featureUsageRepository;
    }
    
    public async Task<Result> TryIncrementUsageAsync(Guid userId, FeatureName featureName, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionRepository
            .GetActiveSubscriptionByUserIdAsync(userId, includePlanPricing: true, cancellationToken: cancellationToken);
        if (subscription is null)
        {
            return Result.Failure([SubscriptionErrors.NotFound]);
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var planFeature = await _planFeatureRepository
            .GetByFeatureNameAsync(subscription.PlanPricing.PlanId, featureName, cancellationToken);
        if (planFeature is null)
        {
            return Result.Failure([PlanFeatureErrors.NotFound]);
        }
        
        var usageStats = await _featureUsageRepository
            .GetUsedFeatureQuotaAsync(userId, featureName, today, cancellationToken);

        var errorList = CheckLimits(featureName, usageStats, planFeature);
        
        if (errorList.Any())
        {
            return Result.Failure<bool>(errorList);
        }

        // If there are any limits defined, increment the usage. Otherwise, the feature is unlimited.
        if (!(planFeature.DailyLimit is null && planFeature.MonthlyLimit is null && planFeature.AbsoluteLimit is null))
        {
            await _featureUsageRepository.IncrementFeatureUsageAsync(userId, featureName, 1, today, cancellationToken);
        }
        return Result.Success();
    }

    public async Task<Result<int?>> GetRemainingUsageAsync(Guid userId, FeatureName featureName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    private List<Error> CheckLimits(FeatureName featureName, FeatureUsageStats usageStats, PlanFeature planFeature)
    {
        var errorList = new List<Error>();
        
        if (planFeature.DailyLimit is not null && planFeature.DailyLimit == 0 ||
            planFeature.MonthlyLimit is not null && planFeature.MonthlyLimit == 0 ||
            planFeature.AbsoluteLimit is not null && planFeature.AbsoluteLimit == 0)
        {
            errorList.Add(FeatureUsageErrors.AccessDenied(featureName));
            return errorList;
        }
        
        if (planFeature.DailyLimit is not null && usageStats.DailyUsage >= planFeature.DailyLimit)
        {
            errorList.Add(FeatureUsageErrors.DailyLimitExceeded(featureName));
        }
        if (planFeature.MonthlyLimit is not null && usageStats.MonthlyUsage >= planFeature.MonthlyLimit)
        {
            errorList.Add(FeatureUsageErrors.MonthlyLimitExceeded(featureName));
        }
        if (planFeature.AbsoluteLimit is not null && usageStats.AbsoluteUsage >= planFeature.AbsoluteLimit)
        {
            errorList.Add(FeatureUsageErrors.AbsoluteLimitExceeded(featureName));
        }

        return errorList;
    }
}