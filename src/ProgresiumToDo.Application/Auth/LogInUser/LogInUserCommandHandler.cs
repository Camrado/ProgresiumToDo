using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Auth.LogInUser;

internal sealed class LogInUserCommandHandler : ICommandHandler<LogInUserCommand, LogInUserCommandResponse>
{
    private readonly IIdentityService _identityService;
    
    public LogInUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    public async Task<Result<LogInUserCommandResponse>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
        var authTokens = await _identityService.LoginAsync(request.Email, request.Password, cancellationToken);
        if (authTokens.IsFailure)
        {
            return Result.Failure<LogInUserCommandResponse>(authTokens.Errors);
        }
        
        return new LogInUserCommandResponse(
            "Login successful.",
            authTokens.Value.AccessToken,
            authTokens.Value.RefreshToken.Token,
            authTokens.Value.ExpiresIn);
    }
}