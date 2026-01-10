using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Billing.Queries.GetAllPlans;

public sealed record GetAllPlansQuery() : IQuery<GetAllPlansQueryResponse>;