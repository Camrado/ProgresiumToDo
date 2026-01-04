using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Infrastructure.Repositories.Tasks;

internal sealed class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    
    public void Delete(Tag tag)
    {
        DbContext.Tags.Remove(tag);
    }

    public async Task<Tag?> GetByProjectIdAndNameAsync(Guid projectId, string name, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .FirstOrDefaultAsync(tag => tag.ProjectId == projectId && tag.Name == name, cancellationToken);
    }

    public async Task<List<Tag>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .Where(t => t.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tag?> GetByIdAndProjectIdAsync(Guid id, Guid projectId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .FirstOrDefaultAsync(t => t.Id == id && t.ProjectId == projectId, cancellationToken);
    }
}