using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Auth.Tokens;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Auth.Commands.ResetPassword;

internal sealed class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, ResetPasswordCommandResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IRefreshTokenService _refreshTokenService;

    public ResetPasswordCommandHandler(IIdentityService identityService, IRefreshTokenService refreshTokenService)
    {
        _identityService = identityService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<Result<ResetPasswordCommandResponse>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ResetPasswordAsync(request.Email, request.Code, request.NewPassword);
        if (result.IsFailure)
        {
            return Result.Failure<ResetPasswordCommandResponse>(result.Errors);
        }
        
        var revokeResult = await _refreshTokenService.RevokeAllRefreshTokensAsync(request.Email, cancellationToken);
        if (revokeResult.IsFailure)
        {
            return Result.Failure<ResetPasswordCommandResponse>(revokeResult.Errors);
        }

        return new ResetPasswordCommandResponse("Password has been reset successfully.");
    }
}
