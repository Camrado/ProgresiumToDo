using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Billing.GetAllPlans;

public sealed record GetAllPlansQuery() : IQuery<GetAllPlansQueryResponse>;