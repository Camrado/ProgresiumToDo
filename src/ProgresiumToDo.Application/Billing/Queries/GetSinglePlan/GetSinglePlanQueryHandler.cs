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

        var planDto = PlanListItemDto.FromDomain(plan);
        
        return Task.FromResult<Result<GetSinglePlanQueryResponse>>(
            new GetSinglePlanQueryResponse("Plan retrieved successfully.", planDto));
    }
}