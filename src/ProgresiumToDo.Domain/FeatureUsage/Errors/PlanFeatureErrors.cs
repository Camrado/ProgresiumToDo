using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.FeatureUsage.Errors;

public static class PlanFeatureErrors
{
    public static Error NotFound => new(
        "PlanFeature.NotFound",
        "The plan feature was not found.");
}