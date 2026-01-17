using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.FeatureUsage;

namespace ProgresiumToDo.Infrastructure.Repositories.Billing;

internal sealed class FeatureUsageRepository : IFeatureUsageRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public FeatureUsageRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FeatureUsageStats> GetUsedFeatureQuotaAsync(Guid userId, FeatureName featureName, DateOnly currentDate,
        CancellationToken cancellationToken = default)
    {
        var firstDayOfMonth = new DateOnly(currentDate.Year, currentDate.Month, 1);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        return await _dbContext.FeatureUsages
            .Where(fu => fu.UserId == userId && fu.Feature.Name == featureName)
            .GroupBy(fu => 1)
            .Select(g => new FeatureUsageStats(
                g.Where(fu => fu.UsageDate == currentDate).Sum(fu => (int?)fu.UsageCount) ?? 0,
                g.Where(fu => fu.UsageDate >= firstDayOfMonth && fu.UsageDate <= lastDayOfMonth)
                    .Sum(fu => (int?)fu.UsageCount) ?? 0,
                g.Sum(fu => (int?)fu.UsageCount) ?? 0))
            .FirstOrDefaultAsync(cancellationToken) ?? new FeatureUsageStats(0, 0, 0);
    }

    public async Task IncrementFeatureUsageAsync(Guid userId, FeatureName featureName, int incrementBy, DateOnly usageDate,
        CancellationToken cancellationToken = default)
    {
        var result = await _dbContext.Features
            .Where(f => f.Name == featureName)
            .Select(f => new
            {
                FeatureId = f.Id,
                Usage = f.FeatureUsages.FirstOrDefault(fu => fu.UserId == userId && fu.UsageDate == usageDate)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            throw new InvalidOperationException($"Feature '{featureName}' is not configured in the database.");
        }

        if (result.Usage is null)
        {
            var featureUsage = FeatureUsage.Create(userId, result.FeatureId, usageDate, incrementBy);
            _dbContext.FeatureUsages.Add(featureUsage);
        }
        else
        {
            result.Usage.IncrementUsage(incrementBy);
        }
    }
}