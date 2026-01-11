using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Billing.Queries.GetSubscriptionsHistory;

public sealed record GetSubscriptionsHistoryQuery() : IQuery<GetSubscriptionsHistoryQueryResponse>;