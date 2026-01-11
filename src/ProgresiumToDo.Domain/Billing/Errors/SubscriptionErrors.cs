using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Billing.Errors;

public static class SubscriptionErrors
{
    public static Error AlreadySubscribedToThisPlan => new(
        "Subscription.AlreadySubscribedToThisPlan",
        "The user is already subscribed to this plan.");
    
    public static Error NoActiveSubscription => new(
        "Subscription.NoActiveSubscription",
        "The user does not have an active subscription.");
}