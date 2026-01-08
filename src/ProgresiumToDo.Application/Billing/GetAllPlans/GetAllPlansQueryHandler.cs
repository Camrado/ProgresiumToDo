using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Billing;

namespace ProgresiumToDo.Application.Billing.GetAllPlans;

internal sealed class GetAllPlansQueryHandler : IQueryHandler<GetAllPlansQuery, GetAllPlansQueryResponse>
{
    private readonly IPlanRepository _planRepository;

    public GetAllPlansQueryHandler(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }
    
    public async Task<Result<GetAllPlansQueryResponse>> Handle(GetAllPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await _planRepository.GetAllWithPricingsAndFeaturesIncludedAsync(cancellationToken);

        var plansDto = plans
            .Select(p => new PlanListItemDto(
                p.Id,
                p.Name,
                p.Description,
                p.PlanFeatures
                    .Select(pf => new FeatureListItemDto(
                        pf.Feature.Name,
                        pf.DailyLimit,
                        pf.MonthlyLimit))
                    .ToList(),
                p.PlanPricings
                    .Select(pp => new PricingListItemDto(
                        pp.Id,
                        pp.Price,
                        pp.BillingPeriod.ToString(),
                        new RegionDto(pp.Region.Code, pp.Region.Currency)))
                    .ToList()
            ));

        return new GetAllPlansQueryResponse("Plans retrieved successfully.", plansDto);
    }
}