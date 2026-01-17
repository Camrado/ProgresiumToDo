namespace ProgresiumToDo.Application.Abstractions.Auth.Entitlement;

public record FeatureUsageStats(
    int DailyUsage,
    int MonthlyUsage,
    int AbsoluteUsage);