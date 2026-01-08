using ProgresiumToDo.Application.Abstractions.Billing;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Billing;

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
        var freePlan = await _planRepository.GeyByNameWithPricingsIncludedAsync("Free", cancellationToken);
        
        if (freePlan is null)
            return Result.Failure([BillingErrors.PlanNotFound]);
        
        var freePlanPricing = freePlan.PlanPricings.FirstOrDefault(pp => pp.BillingPeriod == BillingPeriod.Monthly);
        
        if (freePlanPricing is null)
            return Result.Failure([BillingErrors.PlanPricingNotFound]);

        var subscription = Subscription.Create(
            DateTime.UtcNow, null, true, SubscriptionStatus.Active, userId, freePlanPricing.Id);

        _subscriptionRepository.Add(subscription);
        
        return Result.Success();
    }

    public async Task<Result> SubscribeUserToPlanAsync(Guid userId, Guid planPricingId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}