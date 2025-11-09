namespace ProgresiumToDo.Domain.Auth;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    void Add(RefreshToken refreshToken);
}