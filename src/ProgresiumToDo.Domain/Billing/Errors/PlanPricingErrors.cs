using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Billing.Errors;

public static class PlanPricingErrors
{
    public static Error NotFound => new(
        "PlanPricing.NotFound",
        "The specified plan pricing was not found.");
}