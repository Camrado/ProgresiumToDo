namespace ProgresiumToDo.Application.Billing.Queries.GetAllPlans;

public sealed record GetAllPlansQueryResponse(string Message, IEnumerable<PlanListItemDto> Plans);

public sealed record PlanListItemDto(
    Guid Id,
    string Name,
    string? Description,
    IEnumerable<FeatureListItemDto> Features,
    IEnumerable<PricingListItemDto> Pricings);
    
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