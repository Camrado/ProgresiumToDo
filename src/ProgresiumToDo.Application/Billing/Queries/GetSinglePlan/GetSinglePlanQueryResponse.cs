using ProgresiumToDo.Application.Billing.Queries.GetAllPlans;

namespace ProgresiumToDo.Application.Billing.Queries.GetSinglePlan;

public sealed record GetSinglePlanQueryResponse(string Message, PlanListItemDto Plan);