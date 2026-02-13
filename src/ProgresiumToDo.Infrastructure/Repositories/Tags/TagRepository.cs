using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Domain.Tags;

namespace ProgresiumToDo.Infrastructure.Repositories.Tags;

internal sealed class TagRepository : Repository<Tag>, ITagRepository
{
    public TagRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    
    public void Delete(Tag tag)
    {
        DbContext.Tags.Remove(tag);
    }

    public async Task<Tag?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .FirstOrDefaultAsync(tag => tag.Name == name, cancellationToken);
    }

    public async Task<List<Tag>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .ToListAsync(cancellationToken);
    }

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
    
    public async Task<List<Tag>> GetBySeveralIdsAsync(List<Guid> tagIds, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .Where(t => tagIds.Contains(t.Id))
            .ToListAsync(cancellationToken);
    }
}