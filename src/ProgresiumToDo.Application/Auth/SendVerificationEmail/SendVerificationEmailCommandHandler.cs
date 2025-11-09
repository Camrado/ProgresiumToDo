using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Auth.SendVerificationEmail;

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

        var verificationToken = await _identityService.GenerateEmailVerificationTokenAsync(_userContext.Email);
        if (verificationToken.IsFailure)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>(verificationToken.Errors);
        }
        
        var result = await _emailService.SendEmailAsync(_userContext.Email, "Email Verification",
            $"Please verify your email using this token: {verificationToken.Value}", cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>(result.Errors);
        }
        
        return new SendVerificationEmailCommandResponse("Verification email sent successfully.");
    }
}