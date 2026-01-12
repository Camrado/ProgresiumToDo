using ProgresiumToDo.Application.Abstractions.Billing;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Billing;
using ProgresiumToDo.Domain.Billing.Errors;

namespace ProgresiumToDo.Infrastructure.Billing;

internal sealed class SubscriptionService : ISubscriptionService
{
    private readonly IPlanRepository _planRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    
    public SubscriptionService(IPlanRepository planRepository, ISubscriptionRepository subscriptionRepository)
    {
        _planRepository = planRepository;
        _subscriptionRepository = subscriptionRepository;
    }
    
    public async Task<Result> SubscribeUserToFreePlanAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var freePlan = await _planRepository.GetByNameWithPricingsIncludedAsync(PlanType.Free, cancellationToken);
        
        if (freePlan is null)
            return Result.Failure([PlanErrors.NotFound]);
        
        var freePlanPricing = freePlan.PlanPricings.FirstOrDefault(pp => pp.BillingPeriod == BillingPeriod.Monthly);
        
        if (freePlanPricing is null)
            return Result.Failure([PlanPricingErrors.NotFound]);

        var subscription = Subscription.Create(
            DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), true, userId, freePlanPricing.Id);

        _subscriptionRepository.Add(subscription);
        
        return Result.Success();
    }

    // TODO: Add proration logic for mid-cycle subscriptions
    public async Task<Result<Subscription>> SubscribeUserToPlanAsync(Guid userId, PlanPricing planPricing,
        bool isAutoRenew, CancellationToken cancellationToken = default)
    {
        var existingSubscription =
            await _subscriptionRepository.GetActiveSubscriptionByUserIdAsync(userId, cancellationToken: cancellationToken);
        if (existingSubscription is null)
        {
            return Result.Failure<Subscription>([SubscriptionErrors.AlreadyOnFreePlan]);
        }

        if (existingSubscription.PlanPricingId == planPricing.Id)
        {
            return Result.Failure<Subscription>([SubscriptionErrors.AlreadySubscribedToThisPlan]);
        }

        var newStartDate = DateTime.UtcNow;

        existingSubscription.EndSubscription(newStartDate);

        var newEndDate = planPricing.BillingPeriod == BillingPeriod.Monthly
            ? newStartDate.AddMonths(1)
            : newStartDate.AddYears(1);

        var subscription = Subscription.Create(newStartDate, newEndDate, isAutoRenew, userId, planPricing.Id);

        subscription.FillPlanPricing(planPricing);

        _subscriptionRepository.Add(subscription);

        return subscription;
    }

    public async Task<Result> CancelUserSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existingSubscription = await _subscriptionRepository
            .GetActiveSubscriptionByUserIdAsync(userId, includePlan: true, cancellationToken: cancellationToken);
        if (existingSubscription is null)
        {
            return Result.Failure([SubscriptionErrors.AlreadyOnFreePlan]);
        }

        if (existingSubscription.PlanPricing.Plan.Name == PlanType.Free)
        {
            return Result.Failure([SubscriptionErrors.AlreadyOnFreePlan]);
        }

        var freePlan = await _planRepository.GetByNameWithPricingsIncludedAsync(PlanType.Free, cancellationToken);
        if (freePlan is null)
        {
            return Result.Failure([PlanErrors.NotFound]);
        }
        
        var freePlanPricing = freePlan.PlanPricings.FirstOrDefault(pp => pp.BillingPeriod == BillingPeriod.Monthly);
        if (freePlanPricing is null)
        {
            return Result.Failure([PlanPricingErrors.NotFound]);
        }
        
        var newStartDate = DateTime.UtcNow;
        var newEndDate = newStartDate.AddMonths(1);
        
        existingSubscription.EndSubscription(newStartDate);
        
        var subscription = Subscription.Create(newStartDate, newEndDate, true, userId, freePlanPricing.Id);
        
        _subscriptionRepository.Add(subscription);

        return Result.Success();
    }
}