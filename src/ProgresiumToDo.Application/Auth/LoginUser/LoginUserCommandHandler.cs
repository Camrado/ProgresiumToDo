using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Auth.LoginUser;

internal sealed class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, LoginUserCommandResponse>
{
    private readonly IIdentityService _identityService;
    
    public LoginUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    public async Task<Result<LoginUserCommandResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var authTokens = await _identityService.LoginAsync(request.Email, request.Password, cancellationToken);
        if (authTokens.IsFailure)
        {
            return Result.Failure<LoginUserCommandResponse>(authTokens.Errors);
        }
        
        return new LoginUserCommandResponse(
            "Login successful.",
            authTokens.Value.AccessToken,
            authTokens.Value.RefreshToken.Token,
            authTokens.Value.ExpiresIn);
    }
}