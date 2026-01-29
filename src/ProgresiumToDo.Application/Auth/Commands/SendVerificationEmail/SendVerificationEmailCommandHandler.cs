using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth.Errors;

namespace ProgresiumToDo.Application.Auth.Commands.SendVerificationEmail;

internal sealed class SendVerificationEmailCommandHandler : 
    ICommandHandler<SendVerificationEmailCommand, SendVerificationEmailCommandResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;
    private readonly IEmailService _emailService;
    
    public SendVerificationEmailCommandHandler(IIdentityService identityService, IUserContext userContext, IEmailService emailService)
    {
        _identityService = identityService;
        _userContext = userContext;
        _emailService = emailService;
    }
    
    public async Task<Result<SendVerificationEmailCommandResponse>> Handle(SendVerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
        var isEmailVerified = await _identityService.IsEmailVerifiedAsync(_userContext.Email);
        if (isEmailVerified.IsFailure)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>(isEmailVerified.Errors);
        }
        if (isEmailVerified.Value)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>([UserErrors.EmailAlreadyVerified]);
        }

        var verificationUrl = await _identityService.GenerateEmailVerificationUrlAsync(_userContext.Email);
        if (verificationUrl.IsFailure)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>(verificationUrl.Errors);
        }
        
        var result = await _emailService.SendEmailAsync(_userContext.Email, "Email Verification",
            $"Please verify your email: {verificationUrl.Value}", cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>(result.Errors);
        }

        return new SendVerificationEmailCommandResponse("Verification email sent successfully.",
            verificationUrl.Value); // TODO: Remove VerificationUrl from response after implementing email service
    }
}