using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Auth.LoginUser;
using ProgresiumToDo.Application.Auth.RefreshTokens;
using ProgresiumToDo.Application.Auth.RegisterUser;

namespace ProgresiumToDo.API.Controllers.Auth;

[Route("api/progresium-todo/v1/auth")]
public class AuthController : ApiControllerBase
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
        return FromResult(result);
    } 
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand loginUserCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(loginUserCommand, cancellationToken);
        return FromResult(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-tokens")]
    public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokensCommand refreshTokensCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(refreshTokensCommand, cancellationToken);
        return FromResult(result);
    }
}