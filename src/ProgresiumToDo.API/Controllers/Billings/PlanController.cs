using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Billing.Queries.GetAllPlans;
using ProgresiumToDo.Application.Billing.Queries.GetSinglePlan;

namespace ProgresiumToDo.API.Controllers.Billings;

[Route("api/progresium-todo/v1/plans")]
public class PlanController : ApiControllerBase
{
    public PlanController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPlans(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetAllPlansQuery(), cancellationToken);
        return FromResult(result);
    }

    [HttpGet("{planId:guid}")]
    public async Task<IActionResult> GetPlanById([FromRoute] GetSinglePlanQuery getSinglePlanQuery,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(getSinglePlanQuery, cancellationToken);
        return FromResult(result);
    }
}