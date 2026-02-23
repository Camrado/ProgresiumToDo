using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

    public async Task<Tag?> GetByNameAsync(string name, Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Tag> query = DbContext.Tags;

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .FirstOrDefaultAsync(t => t.Name == name && t.UserId == userId, cancellationToken);
    }

    public async Task<List<Tag>> GetAllAsync(Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Tag> query = DbContext.Tags;

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .Where(t => t.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Tag>> GetByNamesAsync(List<string> names, Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Tag> query = DbContext.Tags;

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .Where(t => names.Contains(t.Name) && t.UserId == userId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Tag?> GetByIdAndUserIdAsync(Guid tagId, Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<Tag> query = DbContext.Tags;

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .FirstOrDefaultAsync(t => t.Id == tagId && t.UserId == userId, cancellationToken);
    }
}