using ProgresiumToDo.Domain.FeatureUsage;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.FeatureUsage;

public class PlanFeatureTests
{
    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        var planId = Guid.NewGuid();
        var featureId = Guid.NewGuid();
        int? dailyLimit = 10;
        int? monthlyLimit = 100;
        int? absoluteLimit = 1000;

        var planFeature = PlanFeature.Create(planId, featureId, dailyLimit, monthlyLimit, absoluteLimit);

        planFeature.ShouldNotBeNull();
        planFeature.PlanId.ShouldBe(planId);
        planFeature.FeatureId.ShouldBe(featureId);
        planFeature.DailyLimit.ShouldBe(dailyLimit);
        planFeature.MonthlyLimit.ShouldBe(monthlyLimit);
        planFeature.AbsoluteLimit.ShouldBe(absoluteLimit);
    }

    [Fact]
    public void Create_WithNullLimits_Should_SetNulls()
    {
        var planFeature = PlanFeature.Create(Guid.NewGuid(), Guid.NewGuid(), null, null, null);

        planFeature.DailyLimit.ShouldBeNull();
        planFeature.MonthlyLimit.ShouldBeNull();
        planFeature.AbsoluteLimit.ShouldBeNull();
    }
}
