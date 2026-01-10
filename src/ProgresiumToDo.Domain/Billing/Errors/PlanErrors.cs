using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Billing.Errors;

public static class PlanErrors
{
    public static Error NotFound => new(
        "Plan.NotFound",
        "The specified billing plan was not found.");
}