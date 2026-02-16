using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Infrastructure.Repositories;

public abstract class Repository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext DbContext;
    
    protected Repository(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public virtual async Task<T?> GetByIdAsync(Guid id, bool trackChanges = false, CancellationToken cancellationToken = default) 
    {
        IQueryable<T> query = DbContext.Set<T>();

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(entity => entity.Id == id, cancellationToken);
    }
    
    public virtual void Add(T entity) 
    {
        DbContext.Add(entity);
    }
}