using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Auth.Commands.SendForgotPasswordEmail;

internal sealed class SendForgotPasswordEmailCommandHandler : 
    ICommandHandler<SendForgotPasswordEmailCommand, SendForgotPasswordEmailCommandResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public SendForgotPasswordEmailCommandHandler(IIdentityService identityService,
        IEmailService emailService, IUserRepository userRepository)
    {
        _identityService = identityService;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task<Result<SendForgotPasswordEmailCommandResponse>> Handle(SendForgotPasswordEmailCommand request,
        CancellationToken cancellationToken)
    {
        var doesUserExist = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken: cancellationToken);
        if (!doesUserExist)
        {
            // Return success to prevent email enumeration
            return new SendForgotPasswordEmailCommandResponse("If an account with that email exists, a password reset code has been sent.");
        }

        var resetCode = await _identityService.GeneratePasswordResetCodeAsync(request.Email);
        if (resetCode.IsFailure)
        {
            return Result.Failure<SendForgotPasswordEmailCommandResponse>(resetCode.Errors);
        }

        var result = await _emailService.SendPasswordResetEmailAsync(request.Email, resetCode.Value, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<SendForgotPasswordEmailCommandResponse>(result.Errors);
        }
        
        return new SendForgotPasswordEmailCommandResponse("If an account with that email exists, a password reset code has been sent.");
    }
}
