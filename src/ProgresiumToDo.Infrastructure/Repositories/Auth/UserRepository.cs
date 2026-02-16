using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Infrastructure.Repositories.Auth;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    
    public UserRepository(ApplicationDbContext dbContext, ILogger<UserRepository> logger) : base(dbContext)
    {
        _logger = logger;
    }

    public async Task AcquireUserLockAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Fail fast if someone calls this without a transaction.
        if (DbContext.Database.CurrentTransaction is null)
        {
            _logger.LogError(
                "AcquireUserLockAsync called without an active transaction. UserId: {UserId}",
                userId);
            throw new InvalidOperationException(
                "AcquireUserLockAsync requires an active transaction to function correctly.");
        }
        
        await DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT 1 FROM users WHERE id = {userId} FOR UPDATE", 
            cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = DbContext.Set<User>();

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>().AnyAsync(u => u.Email == email, cancellationToken);
    }
    
    public void Delete(User user)
    {
        DbContext.Set<User>().Remove(user);
    }
    
    public async Task<bool> IsEmailVerifiedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => u.IsEmailVerified)
            .FirstOrDefaultAsync(cancellationToken);
    }
}