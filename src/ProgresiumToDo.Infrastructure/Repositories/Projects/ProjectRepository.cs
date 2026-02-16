using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Projects.Repositories;
using ProgresiumToDo.Domain.Projects;

namespace ProgresiumToDo.Infrastructure.Repositories.Projects;

internal sealed class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Project?> GetByNameAndUserIdAsync(string name, Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Project> query = DbContext.Set<Project>();

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .FirstOrDefaultAsync(p => p.Name == name && p.UserId == userId, cancellationToken);
    }

    public async Task<Project?> GetByIdAndUserIdAsync(Guid projectId, Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Project> query = DbContext.Set<Project>();

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId, cancellationToken);
    }

    public async Task<List<Project>> GetAllByUserIdAsync(Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Project> query = DbContext.Set<Project>();

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .Where(p => p.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public void Delete(Project project)
    {
        DbContext.Set<Project>().Remove(project);
    }
}