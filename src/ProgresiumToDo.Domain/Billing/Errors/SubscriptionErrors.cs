using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Domain.Billing.Errors;

public static class SubscriptionErrors
{
    public static Error AlreadySubscribedToThisPlan => new(
        "Subscription.AlreadySubscribedToThisPlan",
        "The user is already subscribed to this plan.");
    
    public static Error AlreadyOnFreePlan => new(
        "Subscription.AlreadyOnFreePlan",
        "The user is already on the free plan.");
    
    public static Error NotFound => new(
        "Subscription.NotFound",
        "The subscription was not found.");
}