using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Billing.Commands.CancelSubscription;

public sealed record CancelSubscriptionCommand() : ICommand<CancelSubscriptionCommandResponse>;