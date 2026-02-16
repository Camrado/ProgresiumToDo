using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Auth.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth.Errors;

namespace ProgresiumToDo.Application.Auth.Commands.LogOutUser;

internal sealed class LogOutUserCommandHandler : ICommandHandler<LogOutUserCommand, LogOutUserCommandResponse>
{
    private readonly IUserContext _userContext;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogOutUserCommandHandler(IUserContext userContext, IRefreshTokenRepository refreshTokenRepository)
    {
        _userContext = userContext;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result<LogOutUserCommandResponse>> Handle(LogOutUserCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, trackChanges: true, cancellationToken);
        if (token is null || token.UserId != _userContext.UserId)
        {
            return Result.Failure<LogOutUserCommandResponse>([RefreshTokenErrors.InvalidToken]);
        }   
        
        token.Revoke();

        return new LogOutUserCommandResponse("Logout successful.");
    }
}