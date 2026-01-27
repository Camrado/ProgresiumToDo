using Microsoft.Extensions.Logging;
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
    private readonly ILogger<SubscriptionService> _logger;
    
    public SubscriptionService(
        IPlanRepository planRepository,
        ISubscriptionRepository subscriptionRepository,
        ILogger<SubscriptionService> logger)
    {
        _planRepository = planRepository;
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
    }
    
    public async Task<Result> SubscribeUserToFreePlanAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var freePlan = await _planRepository.GetByNameWithPricingsIncludedAsync(PlanType.Free, cancellationToken);
        
        if (freePlan is null)
        {
            _logger.LogCritical("CRITICAL: Failed to subscribe user to free plan. Free plan not found. UserId: {UserId}", userId);
            return Result.Failure([PlanErrors.NotFound]);
        }
        
        var freePlanPricing = freePlan.PlanPricings.FirstOrDefault(pp => pp.BillingPeriod == BillingPeriod.Monthly);
        
        if (freePlanPricing is null)
        {
            _logger.LogCritical(
                "CRITICAL: Failed to subscribe user to free plan. Free plan pricing not found. UserId: {UserId}, PlanId: {PlanId}",
                userId,
                freePlan.Id);
            return Result.Failure([PlanPricingErrors.NotFound]);
        }

        var subscription = Subscription.Create(
            DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), true, userId, freePlanPricing.Id);

        _subscriptionRepository.Add(subscription);
        
        _logger.LogInformation(
            "User subscribed to free plan. UserId: {UserId}, PlanId: {PlanId}, SubscriptionId: {SubscriptionId}",
            userId,
            freePlan.Id,
            subscription.Id);
        
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
            _logger.LogWarning(
                "Failed to subscribe user to plan. No active subscription found. UserId: {UserId}",
                userId);
            return Result.Failure<Subscription>([SubscriptionErrors.NotFound]);
        }

        if (existingSubscription.PlanPricingId == planPricing.Id)
        {
            _logger.LogInformation(
                "Failed to subscribe user to plan. User already subscribed to this plan. UserId: {UserId}, PlanPricingId: {PlanPricingId}",
                userId,
                planPricing.Id);
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
        
        _logger.LogInformation(
            "User subscribed to paid plan. UserId: {UserId}, OldPlanPricingId: {OldPlanPricingId}, NewPlanPricingId: {NewPlanPricingId}, BillingPeriod: {BillingPeriod}, IsAutoRenew: {IsAutoRenew}",
            userId,
            existingSubscription.PlanPricingId,
            planPricing.Id,
            planPricing.BillingPeriod,
            isAutoRenew);

        return subscription;
    }

    public async Task<Result> CancelUserSubscriptionAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var existingSubscription = await _subscriptionRepository
            .GetActiveSubscriptionByUserIdAsync(userId, includePlan: true, includeRegion: true, cancellationToken: cancellationToken);
        if (existingSubscription is null)
        {
            _logger.LogWarning(
                "Failed to cancel subscription. No active subscription found. UserId: {UserId}",
                userId);
            return Result.Failure([SubscriptionErrors.NotFound]);
        }

        if (existingSubscription.PlanPricing.Plan.Name == PlanType.Free)
        {
            _logger.LogInformation(
                "Failed to cancel subscription. User already on free plan. UserId: {UserId}",
                userId);
            return Result.Failure([SubscriptionErrors.AlreadyOnFreePlan]);
        }

        var freePlan = await _planRepository.GetByNameWithPricingsIncludedAsync(PlanType.Free, cancellationToken);
        if (freePlan is null)
        {
            _logger.LogCritical(
                "CRITICAL: Failed to cancel subscription. Free plan not found. UserId: {UserId}",
                userId);
            return Result.Failure([PlanErrors.NotFound]);
        }
        
        var freePlanPricing = freePlan.PlanPricings.FirstOrDefault(pp => pp.BillingPeriod == BillingPeriod.Monthly);
        if (freePlanPricing is null)
        {
            _logger.LogCritical(
                "CRITICAL: Failed to cancel subscription. Free plan pricing not found. UserId: {UserId}",
                userId);
            return Result.Failure([PlanPricingErrors.NotFound]);
        }
        
        var cancelledPlanId = existingSubscription.PlanPricing.Plan.Id;
        var newStartDate = DateTime.UtcNow;
        var newEndDate = newStartDate.AddMonths(1);
        
        existingSubscription.EndSubscription(newStartDate);
        
        var subscription = Subscription.Create(newStartDate, newEndDate, true, userId, freePlanPricing.Id);
        
        _subscriptionRepository.Add(subscription);
        
        _logger.LogInformation(
            "Subscription cancelled. User downgraded to free plan. UserId: {UserId}, CancelledPlanId: {CancelledPlanId}",
            userId,
            cancelledPlanId);

        return Result.Success();
    }
}