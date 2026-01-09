using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.SubscribeToPlan;

public sealed record SubscribeToPlanCommand(
    Guid PlanPricingId,
    bool IsAutoRenew) : ICommand<SubscribeToPlanCommandResponse>
{
    internal PlanPricing PlanPricing { get; set; }
}