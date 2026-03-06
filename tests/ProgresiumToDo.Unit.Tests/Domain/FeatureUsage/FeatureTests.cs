using ProgresiumToDo.Domain.FeatureUsage;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.FeatureUsage;

public class FeatureTests
{
    [Theory]
    [InlineData(FeatureName.None)]
    [InlineData(FeatureName.TaskDuration)]
    public void Create_Should_SetPropertyValues(FeatureName name)
    {
        var feature = Feature.Create(name, "Test description");

        feature.ShouldNotBeNull();
        feature.Name.ShouldBe(name);
        feature.Description.ShouldBe("Test description");
    }

    [Fact]
    public void Create_WithNullDescription_Should_SetNull()
    {
        var feature = Feature.Create(FeatureName.TaskDuration);

        feature.Description.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var feature = Feature.Create(FeatureName.TaskDuration);

        feature.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_InitializeEmptyCollections()
    {
        var feature = Feature.Create(FeatureName.TaskDuration);

        feature.FeatureUsages.ShouldNotBeNull();
        feature.FeatureUsages.ShouldBeEmpty();
        feature.PlanFeatures.ShouldNotBeNull();
        feature.PlanFeatures.ShouldBeEmpty();
    }
}
