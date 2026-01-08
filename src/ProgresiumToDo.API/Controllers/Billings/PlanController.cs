using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Billing.GetAllPlans;

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
}