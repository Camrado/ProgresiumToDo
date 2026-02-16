using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Auth.Repositories;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Infrastructure.Repositories.Auth;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public RefreshTokenRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<RefreshToken?> GetByTokenAsync(string token, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<RefreshToken> query = _dbContext.RefreshTokens;

        if (!trackChanges)
            query = query.AsNoTracking();
        
        return await query
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<RefreshToken> query = _dbContext.RefreshTokens;

        if (!trackChanges)
            query = query.AsNoTracking();
        
        return await query.Where(rt => rt.UserId == userId).ToListAsync(cancellationToken);
    }

    public void Add(RefreshToken refreshToken)
    {
        _dbContext.Add(refreshToken);
    }
}