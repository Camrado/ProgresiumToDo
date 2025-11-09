using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgresiumToDo.Application.Auth.LogInUser;
using ProgresiumToDo.Application.Auth.LogOutUser;
using ProgresiumToDo.Application.Auth.RefreshTokens;
using ProgresiumToDo.Application.Auth.RegisterUser;
using ProgresiumToDo.Application.Auth.SendVerificationEmail;
using ProgresiumToDo.Application.Auth.VerifyEmail;

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
    public async Task<IActionResult> Login([FromBody] LogInUserCommand logInUserCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(logInUserCommand, cancellationToken);
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

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogOutUserCommand logOutUserCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(logOutUserCommand, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand verifyEmailCommand,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(verifyEmailCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpPost("send-verification-email")]
    public async Task<IActionResult> SendVerificationEmail(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SendVerificationEmailCommand(), cancellationToken);
        return FromResult(result);
    }
}