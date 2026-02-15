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

    public async Task<Tag?> GetByNameAsync(string name, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .FirstOrDefaultAsync(t => t.Name == name && t.UserId == userId, cancellationToken);
    }

    public async Task<List<Tag>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Tag>> GetByNamesAsync(List<string> names, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .Where(t => names.Contains(t.Name) && t.UserId == userId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Tag?> GetByIdAndUserIdAsync(Guid tagId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Tags
            .FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == userId, cancellationToken);
    }
}