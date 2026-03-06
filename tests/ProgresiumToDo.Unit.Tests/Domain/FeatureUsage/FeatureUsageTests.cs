using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.FeatureUsage;

public class FeatureUsageTests
{
    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        var userId = Guid.NewGuid();
        var featureId = Guid.NewGuid();
        var usageDate = new DateOnly(2026, 3, 6);
        var usageCount = 5;

        var usage = ProgresiumToDo.Domain.FeatureUsage.FeatureUsage.Create(
            userId, featureId, usageDate, usageCount);

        usage.ShouldNotBeNull();
        usage.UserId.ShouldBe(userId);
        usage.FeatureId.ShouldBe(featureId);
        usage.UsageDate.ShouldBe(usageDate);
        usage.UsageCount.ShouldBe(usageCount);
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var usage = ProgresiumToDo.Domain.FeatureUsage.FeatureUsage.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), 1);

        usage.Id.ShouldNotBe(Guid.Empty);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void IncrementUsage_Should_IncreaseUsageCount(int incrementBy)
    {
        var usage = ProgresiumToDo.Domain.FeatureUsage.FeatureUsage.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), 0);

        usage.IncrementUsage(incrementBy);

        usage.UsageCount.ShouldBe(incrementBy);
    }

    [Fact]
    public void IncrementUsage_MultipleTimes_Should_AccumulateCount()
    {
        var usage = ProgresiumToDo.Domain.FeatureUsage.FeatureUsage.Create(
            Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow), 10);

        usage.IncrementUsage(3);
        usage.IncrementUsage(7);

        usage.UsageCount.ShouldBe(20);
    }
}
