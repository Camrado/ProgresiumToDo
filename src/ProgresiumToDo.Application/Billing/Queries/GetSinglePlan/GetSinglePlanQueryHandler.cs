using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Billing.Queries.GetAllPlans;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Billing.Queries.GetSinglePlan;

internal sealed class GetSinglePlanQueryHandler : IQueryHandler<GetSinglePlanQuery, GetSinglePlanQueryResponse>
{
    private readonly IPlanRepository _planRepository;
    
    public GetSinglePlanQueryHandler(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }
    
    public Task<Result<GetSinglePlanQueryResponse>> Handle(GetSinglePlanQuery request, CancellationToken cancellationToken)
    {
        var plan = request.Plan!;

        var planDto = new PlanListItemDto(
            plan.Id,
            plan.Name,
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
        
        return Task.FromResult<Result<GetSinglePlanQueryResponse>>(
            new GetSinglePlanQueryResponse("Plan retrieved successfully.", planDto));
    }
}