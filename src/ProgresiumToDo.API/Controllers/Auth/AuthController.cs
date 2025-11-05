using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Auth.RegisterUser;

namespace ProgresiumToDo.API.Controllers.Auth;

[ApiController]
[Route("api/progresium-todo/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand registerUserCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(registerUserCommand, cancellationToken);
        if (result.IsFailure)
        {
            return BadRequest(result.Errors);
        }
        return Ok(result.Value);
    } 
}