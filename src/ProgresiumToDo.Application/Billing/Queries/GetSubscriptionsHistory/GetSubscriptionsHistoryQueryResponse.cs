using ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;

namespace ProgresiumToDo.Application.Billing.Queries.GetSubscriptionsHistory;

public sealed record GetSubscriptionsHistoryQueryResponse(string Message, IEnumerable<SubscriptionDto> Subscriptions);