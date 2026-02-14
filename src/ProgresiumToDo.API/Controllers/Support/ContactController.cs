using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ProgresiumToDo.API.Extensions;
using ProgresiumToDo.Application.Support.Commands.ContactUs;

namespace ProgresiumToDo.API.Controllers.Support;

[Route("api/progresium-todo/v1/contact-us")]
public class ContactController : ApiControllerBase
{
    public ContactController(IMediator mediator) : base(mediator)
    {
    }

    [AllowAnonymous]
    [HttpPost]
    [EnableRateLimiting(RateLimitingExtensions.ContactUsPolicyName)]
    public async Task<IActionResult> JoinWaitlist([FromBody] ContactUsCommand contactUsCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(contactUsCommand, cancellationToken);
        return FromResult(result);
    }
}