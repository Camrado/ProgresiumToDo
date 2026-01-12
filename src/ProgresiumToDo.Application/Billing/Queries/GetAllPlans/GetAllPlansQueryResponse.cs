using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.Queries.GetAllPlans;

public sealed record GetAllPlansQueryResponse(string Message, IEnumerable<PlanListItemDto> Plans);

public sealed record PlanListItemDto(
    Guid Id,
    string Name,
    string? Description,
    IEnumerable<FeatureListItemDto> Features,
    IEnumerable<PricingListItemDto> Pricings)
{
    public static PlanListItemDto FromDomain(Plan plan)
    {
        return new PlanListItemDto(
            plan.Id,
            plan.Name.ToString(),
            plan.Description,
            plan.PlanFeatures
                .Select(pf => new FeatureListItemDto(
                    pf.Feature.Name,
                    pf.DailyLimit,
                    pf.MonthlyLimit))
                .ToList(),
            plan.PlanPricings
                .Select(pp => new PricingListItemDto(
                    pp.Id,
                    pp.Price,
                    pp.BillingPeriod.ToString(),
                    new RegionDto(pp.Region.Code, pp.Region.Currency)))
                .ToList()
        );
    }
}
    
public sealed record FeatureListItemDto(
    string Name,
    int? DailyLimit,
    int? MonthlyLimit);
    
public sealed record PricingListItemDto(
    Guid Id,
    decimal Price,
    string BillingPeriod,
    RegionDto Region
    );
    
public sealed record RegionDto(string Code, string Currency);