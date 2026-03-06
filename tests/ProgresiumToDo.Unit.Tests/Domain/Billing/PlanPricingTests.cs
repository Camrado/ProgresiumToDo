using ProgresiumToDo.Domain.Billing;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Billing;

public class PlanPricingTests
{
    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        var price = 9.99m;
        var period = BillingPeriod.Monthly;
        var planId = Guid.NewGuid();
        var regionId = Guid.NewGuid();

        var pricing = PlanPricing.Create(price, period, planId, regionId);

        pricing.ShouldNotBeNull();
        pricing.Price.ShouldBe(price);
        pricing.BillingPeriod.ShouldBe(period);
        pricing.PlanId.ShouldBe(planId);
        pricing.RegionId.ShouldBe(regionId);
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var pricing = PlanPricing.Create(10m, BillingPeriod.Yearly, Guid.NewGuid(), Guid.NewGuid());

        pricing.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_SetCreatedAtAndUpdatedAt()
    {
        var before = DateTime.UtcNow;

        var pricing = PlanPricing.Create(10m, BillingPeriod.Yearly, Guid.NewGuid(), Guid.NewGuid());

        pricing.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        pricing.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
    }

    [Fact]
    public void Create_Should_InitializeEmptySubscriptions()
    {
        var pricing = PlanPricing.Create(10m, BillingPeriod.Yearly, Guid.NewGuid(), Guid.NewGuid());

        pricing.Subscriptions.ShouldNotBeNull();
        pricing.Subscriptions.ShouldBeEmpty();
    }
}
