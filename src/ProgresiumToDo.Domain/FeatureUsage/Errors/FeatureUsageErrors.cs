using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.FeatureUsage.Errors;

public static class FeatureUsageErrors
{
    public static Error DailyLimitExceeded(FeatureName featureName) => new(
        "FeatureUsage.DailyLimitExceeded",
        $"The daily limit for '{featureName.ToString()}' feature has been exceeded.");
    
    public static Error MonthlyLimitExceeded(FeatureName featureName) => new(
        "FeatureUsage.MonthlyLimitExceeded",
        $"The monthly limit for '{featureName.ToString()}' feature has been exceeded.");
    
    public static Error AbsoluteLimitExceeded(FeatureName featureName) => new(
        "FeatureUsage.AbsoluteLimitExceeded",
        $"The absolute limit for '{featureName.ToString()}' feature has been exceeded.");
    
    public static Error AccessDenied(FeatureName featureName) => new(
        "FeatureUsage.AccessDenied",
        $"The '{featureName.ToString()}' feature is restricted under your current plan.");
}