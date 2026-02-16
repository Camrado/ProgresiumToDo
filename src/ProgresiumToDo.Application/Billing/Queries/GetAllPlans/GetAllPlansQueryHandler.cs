using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Billing.Queries.GetAllPlans;

internal sealed class GetAllPlansQueryHandler : IQueryHandler<GetAllPlansQuery, GetAllPlansQueryResponse>
{
    private readonly IPlanRepository _planRepository;

    public GetAllPlansQueryHandler(IPlanRepository planRepository)
    {
        _planRepository = planRepository;
    }
    
    public async Task<Result<GetAllPlansQueryResponse>> Handle(GetAllPlansQuery request, CancellationToken cancellationToken)
    {
        var plans = await _planRepository.GetAllWithPricingsAndFeaturesIncludedAsync(cancellationToken: cancellationToken);

        var plansDto = plans.Select(PlanListItemDto.FromDomain).ToList();

        return new GetAllPlansQueryResponse("Plans retrieved successfully.", plansDto);
    }
}