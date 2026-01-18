using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Infrastructure.Repositories.Auth;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task AcquireUserLockAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Fail fast if someone calls this without a transaction.
        if (DbContext.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException(
                "AcquireUserLockAsync requires an active transaction to function correctly.");
        }
        
        await DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT 1 FROM users WHERE id = {userId} FOR UPDATE", 
            cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<User>().AnyAsync(u => u.Email == email, cancellationToken);
    }
    
    public void Delete(User user)
    {
        DbContext.Set<User>().Remove(user);
    }
}