using Microsoft.Extensions.Logging;
using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Application.Users.Repositories;
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
    private readonly IUserRepository _userRepository;
    private readonly ILogger<EntitlementService> _logger;
    
    public EntitlementService(
        ISubscriptionRepository subscriptionRepository,
        IPlanFeatureRepository planFeatureRepository,
        IFeatureUsageRepository featureUsageRepository,
        IUserRepository userRepository,
        ILogger<EntitlementService> logger)
    {
        _subscriptionRepository = subscriptionRepository;
        _planFeatureRepository = planFeatureRepository;
        _featureUsageRepository = featureUsageRepository;
        _userRepository = userRepository;
        _logger = logger;
    }
    
    public async Task<Result> TryIncrementUsageAsync(Guid userId, FeatureName featureName, CancellationToken cancellationToken = default)
    {
        // Acquire a user-level lock to prevent race conditions
        await _userRepository.AcquireUserLockAsync(userId, cancellationToken);
        
        var subscription = await _subscriptionRepository
            .GetActiveSubscriptionByUserIdAsync(userId, includePlanPricing: true, cancellationToken: cancellationToken);
        if (subscription is null)
        {
            _logger.LogWarning(
                "Entitlement check failed. No active subscription found. UserId: {UserId}, FeatureName: {FeatureName}",
                userId,
                featureName);
            return Result.Failure([SubscriptionErrors.NotFound]);
        }
        
        var planFeature = await _planFeatureRepository
            .GetByFeatureNameAsync(subscription.PlanPricing.PlanId, featureName, cancellationToken);
        if (planFeature is null)
        {
            _logger.LogWarning(
                "Entitlement check failed. Plan feature not found. UserId: {UserId}, PlanId: {PlanId}, FeatureName: {FeatureName}",
                userId,
                subscription.PlanPricing.PlanId,
                featureName);
            return Result.Failure([PlanFeatureErrors.NotFound]);
        }
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var usageStats = await _featureUsageRepository.GetUsedFeatureQuotaAsync(
            userId, featureName, today, GetFirstDayOfSubscriptionMonth(today, subscription.StartDate), cancellationToken);

        var errorList = CheckLimits(featureName, usageStats, planFeature);
        
        if (errorList.Any())
        {
            var limitTypes = string.Join(", ", errorList.Select(e => e.Code));
            _logger.LogWarning(
                "Entitlement check failed. Limit exceeded. UserId: {UserId}, FeatureName: {FeatureName}, LimitTypes: {LimitTypes}, DailyUsage: {DailyUsage}, MonthlyUsage: {MonthlyUsage}, AbsoluteUsage: {AbsoluteUsage}",
                userId,
                featureName,
                limitTypes,
                usageStats.DailyUsage,
                usageStats.MonthlyUsage,
                usageStats.AbsoluteUsage);
            return Result.Failure(errorList);
        }

        // If there are any limits defined, increment the usage. Otherwise, the feature is unlimited.
        if (!(planFeature.DailyLimit is null && planFeature.MonthlyLimit is null && planFeature.AbsoluteLimit is null))
        {
            await _featureUsageRepository.IncrementFeatureUsageAsync(
                userId, featureName, 1, DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);
            
            _logger.LogInformation(
                "Feature usage incremented. UserId: {UserId}, FeatureName: {FeatureName}, NewDailyUsage: {NewDailyUsage}, NewMonthlyUsage: {NewMonthlyUsage}",
                userId,
                featureName,
                usageStats.DailyUsage + 1,
                usageStats.MonthlyUsage + 1);
        }
        else
        {
            _logger.LogInformation(
                "Feature access granted (unlimited). UserId: {UserId}, FeatureName: {FeatureName}",
                userId,
                featureName);
        }
        return Result.Success();
    }

    public async Task<Result<UserEntitlementSummary>> GetUserEntitlementsAsync(Guid userId,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(
            userId, includePlanPricing: true, includePlan: true, cancellationToken: cancellationToken);

        if (subscription is null)
        {
            return Result.Failure<UserEntitlementSummary>([SubscriptionErrors.NotFound]);
        }
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var cycleStartDate = GetFirstDayOfSubscriptionMonth(today, subscription.StartDate);
        var cycleRenewsAt = cycleStartDate.AddMonths(1);

        var planFeatures = await _planFeatureRepository
            .GetByPlanIdAsync(subscription.PlanPricing.PlanId, includeFeature: true, cancellationToken: cancellationToken);

        var usageTasks = new Dictionary<Guid, Task<FeatureUsageStats>>();
        foreach (var pf in planFeatures)
        {
            usageTasks[pf.FeatureId] = _featureUsageRepository.GetUsedFeatureQuotaAsync(
                userId, pf.Feature.Name, today, cycleStartDate, cancellationToken);
        }
        await Task.WhenAll(usageTasks.Values);

        var featureStatuses = new List<FeatureStatus>();
        foreach (var pf in planFeatures)
        {
            var stats = await usageTasks[pf.FeatureId];
            featureStatuses.Add(new FeatureStatus(pf, stats));
        }
        
        return new UserEntitlementSummary(
            subscription.Id,
            subscription.PlanPricing.Plan.Name.ToString(),
            subscription.PlanPricing.Plan.Description,
            cycleStartDate,
            cycleRenewsAt,
            featureStatuses);
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
    
    private DateOnly GetFirstDayOfSubscriptionMonth(DateOnly today, DateTime subscriptionStartDate)
    {
        var daysInCurrentMonth = DateTime.DaysInMonth(today.Year, today.Month);
        var targetDay = Math.Min(subscriptionStartDate.Day, daysInCurrentMonth);

        var hasCycleStartedThisMonth = today.Day >= targetDay;
        
        if (hasCycleStartedThisMonth)
        {
            var maxDays = DateTime.DaysInMonth(today.Year, today.Month);
            return new DateOnly(today.Year, today.Month, Math.Min(subscriptionStartDate.Day, maxDays));
        }
        else
        {
            var lastMonth = today.AddMonths(-1);
            var maxDays = DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month);
            return new DateOnly(lastMonth.Year, lastMonth.Month, Math.Min(subscriptionStartDate.Day, maxDays));
        }
    }
}