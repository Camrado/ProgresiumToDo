using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Users.DeleteAccount;
using ProgresiumToDo.Application.Users.GetCurrentUser;
using ProgresiumToDo.Application.Users.UpdateProfile;

namespace ProgresiumToDo.API.Controllers.Auth;

[Route("api/progresium-todo/v1/users")]
public class UserController : ApiControllerBase
{
    public UserController(IMediator mediator) : base(mediator)
    {
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpPatch("me")]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateProfileCommand updateProfileCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(updateProfileCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpDelete("delete-account")]
    public async Task<IActionResult> DeleteCurrentUser(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteAccountCommand(), cancellationToken);
        return FromResult(result);
    }
}