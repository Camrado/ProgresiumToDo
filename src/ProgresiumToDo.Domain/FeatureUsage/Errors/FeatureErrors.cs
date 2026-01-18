using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.FeatureUsage.Errors;

public static class FeatureErrors
{
    public static Error InvalidFeatureName => new(
        "Feature.InvalidFeatureName",
        "The specified feature name is invalid.");
}