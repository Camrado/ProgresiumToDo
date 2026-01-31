using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Auth.Commands.VerifyEmail;

internal sealed class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand, VerifyEmailCommandResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IUserContext _userContext;
    
    public VerifyEmailCommandHandler(IIdentityService identityService, IUserContext userContext)
    {
        _identityService = identityService;
        _userContext = userContext;
    }
    
    public async Task<Result<VerifyEmailCommandResponse>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.VerifyEmailAsync(_userContext.Email, request.VerificationCode);
        if (result.IsFailure)
        {
            return Result.Failure<VerifyEmailCommandResponse>(result.Errors);
        }
        
        return new VerifyEmailCommandResponse("Email verified successfully.");
    }
}