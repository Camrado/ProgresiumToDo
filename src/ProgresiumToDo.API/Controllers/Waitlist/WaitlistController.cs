using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Waitlist.Commands.JoinWaitlist;

namespace ProgresiumToDo.API.Controllers.Waitlist;

[Route("api/progresium-todo/v1/waitlist")]
public class WaitlistController : ApiControllerBase
{
    public WaitlistController(IMediator mediator) : base(mediator)
    {
    }

    [AllowAnonymous]
    [HttpPost("join")]
    public async Task<IActionResult> JoinWaitlist([FromBody] JoinWaitlistCommand joinWaitlistCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(joinWaitlistCommand, cancellationToken);
        return FromResult(result);
    }
}