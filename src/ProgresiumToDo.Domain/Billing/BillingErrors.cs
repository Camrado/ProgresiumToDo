using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Billing;

public static class BillingErrors
{
    public static Error PlanNotFound => new(
        "Billing.PlanNotFound",
        "The specified billing plan was not found.");
    
    public static Error PlanPricingNotFound => new(
        "Billing.PlanPricingNotFound",
        "The specified plan pricing was not found.");
}