using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Users.GetCurrentUser;
using ProgresiumToDo.Application.Users.UpdateProfile;

namespace ProgresiumToDo.API.Controllers.Auth;

[Route("api/progresium-todo/v1/users")]
public class UserController : ApiControllerBase
{
    private readonly IMediator _mediator;
    
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpPatch("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateProfileCommand updateProfileCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(updateProfileCommand, cancellationToken);
        return FromResult(result);
    }
}