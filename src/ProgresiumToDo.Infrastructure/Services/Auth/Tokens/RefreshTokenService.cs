using ProgresiumToDo.Application.Abstractions.Auth.Tokens;
using ProgresiumToDo.Application.Auth.Repositories;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth.Errors;

namespace ProgresiumToDo.Infrastructure.Services.Auth.Tokens;

internal sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    
    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
    }
    
    public async Task<Result> RevokeAllRefreshTokensAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken: cancellationToken);
        if (user is null)
        {
            return Result.Failure([UserErrors.UserNotFound]);
        }
        
        var activeRefreshTokens = await _refreshTokenRepository.GetAllActiveByUserIdAsync(user.Id, trackChanges: true, cancellationToken);
        foreach (var token in activeRefreshTokens)
        {
            token.Revoke();
        }

        return Result.Success();
    }
}