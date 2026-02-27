using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ProgresiumToDo.API.Extensions.RateLimiting;
using ProgresiumToDo.Application.Auth.Commands.LogInUser;
using ProgresiumToDo.Application.Auth.Commands.LogOutUser;
using ProgresiumToDo.Application.Auth.Commands.RefreshTokens;
using ProgresiumToDo.Application.Auth.Commands.RegisterUser;
using ProgresiumToDo.Application.Auth.Commands.ResetPassword;
using ProgresiumToDo.Application.Auth.Commands.SendForgotPasswordEmail;
using ProgresiumToDo.Application.Auth.Commands.SendVerificationEmail;
using ProgresiumToDo.Application.Auth.Commands.VerifyEmail;

namespace ProgresiumToDo.API.Controllers.Auth;

[Route("api/progresium-todo/v1/auth")]
public class AuthController : ApiControllerBase
{
    public AuthController(IMediator mediator) : base(mediator)
    {
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand registerUserCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(registerUserCommand, cancellationToken);
        return FromResult(result);
    } 
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LogInUserCommand logInUserCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(logInUserCommand, cancellationToken);
        return FromResult(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-tokens")]
    public async Task<IActionResult> RefreshTokens([FromBody] RefreshTokensCommand refreshTokensCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(refreshTokensCommand, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogOutUserCommand logOutUserCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(logOutUserCommand, cancellationToken);
        return FromResult(result);
    }

    [Authorize]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand verifyEmailCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(verifyEmailCommand, cancellationToken);
        return FromResult(result);
    }
    
    [Authorize]
    [HttpPost("send-verification-email")]
    public async Task<IActionResult> SendVerificationEmail(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new SendVerificationEmailCommand(), cancellationToken);
        return FromResult(result);
    }
    
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] SendForgotPasswordEmailCommand sendForgotPasswordEmailCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(sendForgotPasswordEmailCommand, cancellationToken);
        return FromResult(result);
    }
    
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [EnableRateLimiting(RateLimitingExtensions.ResetPasswordPolicyName)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand resetPasswordCommand,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(resetPasswordCommand, cancellationToken);
        return FromResult(result);
    }
}