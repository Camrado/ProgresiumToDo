using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Auth.VerifyEmail;

internal sealed class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand, VerifyEmailCommandResponse>
{
    private readonly IUserContext _userContext;
    private readonly IIdentityService _identityService;
    
    public VerifyEmailCommandHandler(IUserContext userContext, IIdentityService identityService)
    {
        _userContext = userContext;
        _identityService = identityService;
    }
    
    public async Task<Result<VerifyEmailCommandResponse>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.VerifyEmailAsync(_userContext.Email, request.VerificationToken);
        if (result.IsFailure)
        {
            return Result.Failure<VerifyEmailCommandResponse>(result.Errors);
        }
        
        return new VerifyEmailCommandResponse("Email verified successfully.");
    }
}