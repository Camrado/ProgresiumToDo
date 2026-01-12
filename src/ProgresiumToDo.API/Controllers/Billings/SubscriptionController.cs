using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Billing.Commands.CancelSubscription;
using ProgresiumToDo.Application.Billing.Commands.SubscribeToPlan;
using ProgresiumToDo.Application.Billing.Queries.GetSubscriptionsHistory;

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

    [Authorize]
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelSubscription(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new CancelSubscriptionCommand(), cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpGet("history")]
    public async Task<IActionResult> GetSubscriptionsHistory(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetSubscriptionsHistoryQuery(), cancellationToken);
        return FromResult(result);
    }
}