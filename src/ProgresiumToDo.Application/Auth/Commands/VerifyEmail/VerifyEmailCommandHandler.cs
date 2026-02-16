using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth.Errors;

namespace ProgresiumToDo.Application.Auth.Commands.VerifyEmail;

internal sealed class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand, VerifyEmailCommandResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    
    public VerifyEmailCommandHandler(IIdentityService identityService, IUserContext userContext, IUserRepository userRepository)
    {
        _identityService = identityService;
        _userContext = userContext;
        _userRepository = userRepository;
    }
    
    public async Task<Result<VerifyEmailCommandResponse>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, trackChanges: true, cancellationToken);
        if (user is null)
        {
            return Result.Failure<VerifyEmailCommandResponse>([UserErrors.UserNotFound]);
        }
        
        var result = await _identityService.VerifyEmailAsync(user, request.VerificationCode);
        if (result.IsFailure)
        {
            return Result.Failure<VerifyEmailCommandResponse>(result.Errors);
        }
        
        return new VerifyEmailCommandResponse("Email verified successfully.");
    }
}