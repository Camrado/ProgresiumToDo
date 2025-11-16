using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.OAuth.StartOAuth;

namespace ProgresiumToDo.API.Controllers.Auth;

[Route("api/progresium-todo/v1/oauth")]
public class OAuthController : ApiControllerBase
{
    private readonly IMediator _mediator;
    
    public OAuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpGet("start")]
    public async Task<IActionResult> StartOAuth([FromQuery] string provider, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new StartOAuthCommand(provider), cancellationToken);
        return FromResult(result);
    }
}