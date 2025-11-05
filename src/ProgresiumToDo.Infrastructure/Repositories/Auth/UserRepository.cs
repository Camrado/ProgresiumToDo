using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Auth;

namespace ProgresiumToDo.Infrastructure.Repositories.Auth;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<User>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }
}