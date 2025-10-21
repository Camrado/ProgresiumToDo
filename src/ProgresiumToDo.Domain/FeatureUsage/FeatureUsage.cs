using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Domain.FeatureUsage;

public sealed class FeatureUsage
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    
    public Guid UserId { get; private set; }
    
    public Guid FeatureId { get; private set; }
    
    public DateOnly UsageDate { get; private set; } // DateOnly.FromDateTime(DateTime.UtcNow);
    
    public int UsageCount { get; private set; }
    
    public User User { get; private set; }
    
    public Feature Feature { get; private set; }
    
    private FeatureUsage(Guid userId, Guid featureId, DateOnly usageDate, int usageCount)
    {
        UserId = userId;
        FeatureId = featureId;
        UsageDate = usageDate;
        UsageCount = usageCount;
    }
    
    public static FeatureUsage Create(Guid userId, Guid featureId, DateOnly usageDate, int usageCount)
    {
        return new FeatureUsage(userId, featureId, usageDate, usageCount);
    }
}