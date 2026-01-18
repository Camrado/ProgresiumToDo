using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.FeatureUsage;
using ProgresiumToDo.Infrastructure;

namespace ProgresiumToDo.API.Seeders;

public sealed class FeatureSeeder : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FeatureSeeder> _logger;
    
    public FeatureSeeder(IServiceProvider serviceProvider, ILogger<FeatureSeeder> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var featureNames = Enum.GetValues<FeatureName>();

        var dbFeatures = await dbContext.Features
            .Select(f => f.Name)
            .ToListAsync(cancellationToken);
        
        var missingFeatures = featureNames.Except(dbFeatures).ToList();

        if (!missingFeatures.Any())
            return;

        foreach (var missingFeature in missingFeatures)
        {
            _logger.LogInformation("Seeding missing feature: {FeatureName}", missingFeature.ToString());

            var feature = Feature.Create(missingFeature, $"Auto-seeded feature for {missingFeature.ToString()}");

            dbContext.Add(feature);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}