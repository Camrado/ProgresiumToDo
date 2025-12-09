using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Infrastructure.Repositories.Tasks;

internal sealed class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Project?> GetByNameAndUserIdAsync(string name, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Project>()
            .FirstOrDefaultAsync(p => p.Name == name && p.UserId == userId, cancellationToken);
    }

    public async Task<Project?> GetByIdAndUserIdAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Project>()
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId, cancellationToken);
    }
}