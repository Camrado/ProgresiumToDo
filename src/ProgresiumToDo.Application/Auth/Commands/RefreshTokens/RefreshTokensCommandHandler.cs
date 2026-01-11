using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Auth.Commands.RefreshTokens;

internal sealed class RefreshTokensCommandHandler : ICommandHandler<RefreshTokensCommand, RefreshTokensCommandResponse>
{
    private readonly IIdentityService _identityService;
    
    public RefreshTokensCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    public async Task<Result<RefreshTokensCommandResponse>> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var authTokens = await _identityService.RefreshTokensAsync(request.OldRefreshToken, cancellationToken);
        if (authTokens.IsFailure)
        {
            return Result.Failure<RefreshTokensCommandResponse>(authTokens.Errors);
        }
        
        return new RefreshTokensCommandResponse(
            "Tokens refreshed successfully.",
            authTokens.Value.AccessToken,
            authTokens.Value.RefreshToken.Token,
            authTokens.Value.ExpiresIn);
    }
}