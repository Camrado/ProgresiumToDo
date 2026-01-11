using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Auth.Repositories;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth.Errors;

namespace ProgresiumToDo.Application.Users.Commands.DeleteAccount;

internal sealed class DeleteAccountCommandHandler : ICommandHandler<DeleteAccountCommand, DeleteAccountCommandResponse>
{
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public DeleteAccountCommandHandler(IUserContext userContext, IUserRepository userRepository,
        IIdentityService identityService, IRefreshTokenRepository refreshTokenRepository)
    {
        _userContext = userContext;
        _userRepository = userRepository;
        _identityService = identityService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<Result<DeleteAccountCommandResponse>> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        
        
        var user = await _userRepository.GetByIdAsync(_userContext.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure<DeleteAccountCommandResponse>([UserErrors.UserNotFound]);
        }
        
        _userRepository.Delete(user);
        
        var refreshTokens = await _refreshTokenRepository.GetByUserIdAsync(_userContext.UserId, cancellationToken);
        foreach (var token in refreshTokens)
        {
            token.Revoke();
        }
        
        var result = await _identityService.DeleteAccountAsync(_userContext.Email);
        if (result.IsFailure)
        {
            return Result.Failure<DeleteAccountCommandResponse>(result.Errors);
        }
        
        return new DeleteAccountCommandResponse("Account deleted successfully.");
    }
}