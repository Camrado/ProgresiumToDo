using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;

namespace ProgresiumToDo.API.Controllers.Billings;

[Route("api/progresium-todo/v1/subscription")]
public class SubscriptionController : ApiControllerBase
{
    public SubscriptionController(IMediator mediator) : base(mediator)
    {
    }
    
    [Authorize]
    [HttpPost("subscribe")]
    public async Task<IActionResult> SubscribeToPlan([FromBody] SubscribeToPlanCommand subscribeToPlanCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(subscribeToPlanCommand, cancellationToken);
        return FromResult(result);
    }
}