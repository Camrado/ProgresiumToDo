using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class FeatureUsageRepository : IFeatureUsageRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<FeatureUsageRepository> _logger;
    
    public FeatureUsageRepository(ApplicationDbContext dbContext, ILogger<FeatureUsageRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<FeatureUsageStats> GetUsedFeatureQuotaAsync(Guid userId, FeatureName featureName, DateOnly today, 
        DateOnly firstDayOfSubscriptionMonth, CancellationToken cancellationToken = default)
    {
        var lastDayOfSubscriptionMonth = firstDayOfSubscriptionMonth.AddMonths(1).AddDays(-1);

        return await _dbContext.FeatureUsages
            .Where(fu => fu.UserId == userId && fu.Feature.Name == featureName)
            .GroupBy(fu => 1)
            .Select(g => new FeatureUsageStats(
                g.Where(fu => fu.UsageDate == today).Sum(fu => (int?)fu.UsageCount) ?? 0,
                g.Where(fu => fu.UsageDate >= firstDayOfSubscriptionMonth && fu.UsageDate <= lastDayOfSubscriptionMonth)
                    .Sum(fu => (int?)fu.UsageCount) ?? 0,
                g.Sum(fu => (int?)fu.UsageCount) ?? 0))
            .FirstOrDefaultAsync(cancellationToken) ?? new FeatureUsageStats(0, 0, 0);
    }

    public async Task IncrementFeatureUsageAsync(Guid userId, FeatureName featureName, int incrementBy, DateOnly usageDate,
        CancellationToken cancellationToken = default)
    {
        var featureId = await _dbContext.Features
            .Where(f => f.Name == featureName)
            .Select(f => f.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (featureId == Guid.Empty)
        {
            _logger.LogCritical(
                "CRITICAL: Feature not configured in database. FeatureName: {FeatureName}, UserId: {UserId}",
                featureName,
                userId);
            throw new InvalidOperationException($"Feature '{featureName}' is not configured in the database.");
        }

        // This prevents race conditions on the unique constraint (UserId, FeatureId, UsageDate)
        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"""
             INSERT INTO feature_usages (id, user_id, feature_id, usage_date, usage_count)
             VALUES ({Guid.CreateVersion7()}, {userId}, {featureId}, {usageDate}, {incrementBy})
             ON CONFLICT (user_id, feature_id, usage_date)
                 DO UPDATE SET usage_count = feature_usages.usage_count + excluded.usage_count
             """,
            cancellationToken);
        
        _logger.LogInformation(
            "Feature usage upserted successfully. UserId: {UserId}, FeatureName: {FeatureName}, IncrementBy: {IncrementBy}, UsageDate: {UsageDate}",
            userId,
            featureName,
            incrementBy,
            usageDate);
    }
}