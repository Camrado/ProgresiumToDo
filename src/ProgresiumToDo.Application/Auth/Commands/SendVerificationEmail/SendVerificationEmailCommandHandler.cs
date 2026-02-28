using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Behaviors.Contracts;
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
    private readonly IUnitOfWork _unitOfWork;

    public SendVerificationEmailCommandHandler(IIdentityService identityService, IUserContext userContext,
        IEmailService emailService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _identityService = identityService;
        _userContext = userContext;
        _emailService = emailService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SendVerificationEmailCommandResponse>> Handle(SendVerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken: cancellationToken);
        if (user is null)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>([UserErrors.UserNotFound]);
        }
        
        if (user.IsEmailVerified)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>([UserErrors.EmailAlreadyVerified]);
        }

        var verificationCode = await _identityService.GenerateEmailVerificationCodeAsync(user.Email);
        if (verificationCode.IsFailure)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>(verificationCode.Errors);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = await _emailService.SendVerificationEmailAsync(user.Email, verificationCode.Value, cancellationToken);
        if (result.IsFailure)
        {
            return Result.Failure<SendVerificationEmailCommandResponse>(result.Errors);
        }
        
        return new SendVerificationEmailCommandResponse("Verification email sent successfully.");
    }
}