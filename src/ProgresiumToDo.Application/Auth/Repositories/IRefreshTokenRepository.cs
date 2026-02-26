using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Application.Auth.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetAllByUserIdAsync(Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetAllActiveByUserIdAsync(Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default);
    void Add(RefreshToken refreshToken);
}