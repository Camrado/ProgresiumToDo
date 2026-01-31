using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth.Errors;

namespace ProgresiumToDo.Application.Auth.Commands.SendVerificationEmail;

internal sealed class SendVerificationEmailCommandHandler : 
    ICommandHandler<SendVerificationEmailCommand, SendVerificationEmailCommandResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public SendVerificationEmailCommandHandler(IIdentityService identityService, IUserContext userContext,
        IEmailService emailService, IUserRepository userRepository)
    {
        _identityService = identityService;
        _userContext = userContext;
        _emailService = emailService;
        _userRepository = userRepository;
    }

    public async Task<Result<SendVerificationEmailCommandResponse>> Handle(SendVerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
        var isEmailVerified = await _userRepository.IsEmailVerifiedAsync(_userContext.UserId, cancellationToken);
        if (isEmailVerified)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>([UserErrors.EmailAlreadyVerified]);
        }

        var verificationCode = await _identityService.GenerateEmailVerificationCodeAsync(_userContext.Email);
        if (verificationCode.IsFailure)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>(verificationCode.Errors);
        }
        
        var result = await _emailService.SendConfirmationEmailAsync(
            _userContext.Email, verificationCode.Value, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>(result.Errors);
        }
        
        await _identityService.MarkVerificationEmailAsSentAsync(_userContext.Email);

        return new SendVerificationEmailCommandResponse("Verification email sent successfully.");
    }
}