using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.OAuth.GoogleCallbackOAuth;
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

    [AllowAnonymous]
    [HttpGet("callback/google")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GoogleCallbackOAuthCommand(code, state), cancellationToken);
        return FromResult(result);
    }
}