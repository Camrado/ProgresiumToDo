using ProgresiumToDo.Application.Billing.GetAllPlans;

namespace ProgresiumToDo.Application.Billing.GetSinglePlan;

public sealed record GetSinglePlanQueryResponse(string Message, PlanListItemDto Plan);