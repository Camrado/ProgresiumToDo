using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.GetSinglePlan;

public sealed record GetSinglePlanQuery(Guid PlanId) : IQuery<GetSinglePlanQueryResponse>
{
    internal Plan? Plan { get; set; }
}