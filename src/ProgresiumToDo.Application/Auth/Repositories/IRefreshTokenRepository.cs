using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Auth.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetByUserIdAsync(Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default);
    void Add(RefreshToken refreshToken);
}