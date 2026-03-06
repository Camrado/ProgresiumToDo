using ProgresiumToDo.Domain.Billing;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Billing;

public class SubscriptionTests
{
    private static Subscription CreateDefaultSubscription(
        PlanType? planName = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        bool isAutoRenew = true,
        Guid? userId = null,
        Guid? planPricingId = null)
    {
        return Subscription.Create(
            planName ?? PlanType.Pro,
            startDate ?? DateTime.UtcNow,
            endDate ?? DateTime.UtcNow.AddMonths(1),
            isAutoRenew,
            userId ?? Guid.NewGuid(),
            planPricingId ?? Guid.NewGuid());
    }

    [Fact]
    public void Create_Should_SetPropertyValuesAndDefaultStatus()
    {
        var planName = PlanType.Pro;
        var start = DateTime.UtcNow;
        var end = start.AddMonths(1);
        var autoRenew = true;
        var userId = Guid.NewGuid();
        var pricingId = Guid.NewGuid();

        var sub = Subscription.Create(planName, start, end, autoRenew, userId, pricingId);

        sub.ShouldNotBeNull();
        sub.PlanName.ShouldBe(planName);
        sub.StartDate.ShouldBe(start);
        sub.EndDate.ShouldBe(end);
        sub.IsAutoRenew.ShouldBe(autoRenew);
        sub.UserId.ShouldBe(userId);
        sub.PlanPricingId.ShouldBe(pricingId);
        sub.Status.ShouldBe(SubscriptionStatus.Active);
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var sub = CreateDefaultSubscription();

        sub.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void EndSubscription_Should_UpdateEndDateAndSetStatusToCancelled()
    {
        var sub = CreateDefaultSubscription(endDate: DateTime.UtcNow.AddMonths(1));
        var earlyEnd = DateTime.UtcNow.AddDays(5);

        sub.EndSubscription(earlyEnd);

        sub.EndDate.ShouldBe(earlyEnd);
        sub.Status.ShouldBe(SubscriptionStatus.Cancelled);
    }

    [Fact]
    public void FillPlanPricing_Should_SetPlanPricing_WhenIdMatches()
    {
        var planPricing = PlanPricing.Create(10m, BillingPeriod.Monthly, Guid.NewGuid(), Guid.NewGuid());
        var validSub = Subscription.Create(PlanType.Pro, DateTime.UtcNow, DateTime.UtcNow.AddMonths(1), true, Guid.NewGuid(), planPricing.Id);
        
        validSub.FillPlanPricing(planPricing);

        validSub.PlanPricing.ShouldBe(planPricing);
    }

    [Fact]
    public void FillPlanPricing_Should_ThrowException_WhenIdDoesNotMatch()
    {
        var sub = CreateDefaultSubscription(planPricingId: Guid.NewGuid()); // Some random ID
        var planPricing = PlanPricing.Create(10m, BillingPeriod.Monthly, Guid.NewGuid(), Guid.NewGuid()); // A different generated ID

        Should.Throw<InvalidOperationException>(() => sub.FillPlanPricing(planPricing))
            .Message.ShouldBe("The provided PlanPricing does not match the Subscription's PlanPricingId.");
    }
}
