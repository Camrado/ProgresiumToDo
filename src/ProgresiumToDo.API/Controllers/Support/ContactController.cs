using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Support.Commands.ContactUs;
using ProgresiumToDo.Application.Waitlist.Commands.JoinWaitlist;

namespace ProgresiumToDo.API.Controllers.Support;

[Route("api/progresium-todo/v1/contact-us")]
public class ContactController : ApiControllerBase
{
    public ContactController(IMediator mediator) : base(mediator)
    {
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> JoinWaitlist([FromBody] ContactUsCommand contactUsCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(contactUsCommand, cancellationToken);
        return FromResult(result);
    }
}