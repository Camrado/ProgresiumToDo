using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Abstractions.Auth.Tokens;

public interface IRefreshTokenService
{
    Task<Result> RevokeAllRefreshTokensAsync(string email, CancellationToken cancellationToken = default);
}