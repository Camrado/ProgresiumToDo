using ProgresiumToDo.Domain.Billing;
using Shouldly;

namespace ProgresiumToDo.Unit.Tests.Domain.Billing;

public class RegionTests
{
    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        var code = "US";
        var name = "United States";
        var currency = "USD";

        var region = Region.Create(code, name, currency);

        region.ShouldNotBeNull();
        region.Code.ShouldBe(code);
        region.Name.ShouldBe(name);
        region.Currency.ShouldBe(currency);
    }

    [Fact]
    public void Create_Should_GenerateId()
    {
        var region = Region.Create("EU", "Europe", "EUR");

        region.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void Create_Should_SetCreatedAtAndUpdatedAt()
    {
        var before = DateTime.UtcNow;

        var region = Region.Create("EU", "Europe", "EUR");

        region.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
        region.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
    }

    [Fact]
    public void Create_Should_InitializeEmptyPlanPricings()
    {
        var region = Region.Create("US", "United States", "USD");

        region.PlanPricings.ShouldNotBeNull();
        region.PlanPricings.ShouldBeEmpty();
    }
}
