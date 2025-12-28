using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Tasks;
using ProgresiumToDo.Domain.Tasks.DTOs;

namespace ProgresiumToDo.Infrastructure.Repositories.Tasks;

internal sealed class TaskItemRepository : Repository<TaskItem>, ITaskItemRepository
{
    public TaskItemRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<decimal> GetMaxOrderIndexByProjectId(Guid projectId, DateOnly? dueDate, Guid? parentTaskId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.TaskItems.Where(ti =>
            ti.ProjectId == projectId &&
            ti.DueDate == dueDate && 
            ti.ParentTaskItemId == parentTaskId)
            .MaxAsync(ti => (decimal?)ti.OrderIndex, cancellationToken) ?? 0;
    }

    public async Task<List<TaskItem>> GetAllByUserIdIncludingProjectSubtasksTagsAsync(TaskQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var query = DbContext.TaskItems
            .AsNoTracking()
            .Where(ti => ti.UserId == filter.UserId);

        if (filter.ProjectId.HasValue)
            query = query.Where(ti => ti.ProjectId == filter.ProjectId.Value);
    
        if (filter.DueDateFrom.HasValue)
            query = query.Where(ti => ti.DueDate >= filter.DueDateFrom.Value);
    
        if (filter.DueDateTo.HasValue)
            query = query.Where(ti => ti.DueDate <= filter.DueDateTo.Value);

        if (!string.IsNullOrEmpty(filter.SortBy) && !string.IsNullOrEmpty(filter.SortOrder))
        {
            query = (filter.SortBy.ToLower(), filter.SortOrder.ToLower()) switch
            {
                ("duedate", "asc") => query.OrderBy(ti => ti.DueDate ?? DateOnly.MaxValue),
                ("duedate", "desc") => query.OrderByDescending(ti => ti.DueDate ?? DateOnly.MinValue),
                ("priority", "asc") => query.OrderBy(ti => ti.Priority),
                ("priority", "desc") => query.OrderByDescending(ti => ti.Priority),
                ("createdat", "asc") => query.OrderBy(ti => ti.CreatedAt),
                ("createdat", "desc") => query.OrderByDescending(ti => ti.CreatedAt),
                ("orderindex", "asc") => query.OrderBy(ti => ti.OrderIndex),
                ("orderindex", "desc") => query.OrderByDescending(ti => ti.OrderIndex),
                _ => query.OrderBy(ti => ti.Id)
            };
        }
        else
        {
            query = query.OrderBy(ti => ti.Id);
        }
        
        query = query
            .Include(ti => ti.Project)
            .Include(ti => ti.SubTaskItems)
            .Include(ti => ti.Tags);
        
        if (filter.Page.HasValue && filter.PageSize.HasValue)
        {
            query = query
                .Skip((filter.Page.Value - 1) * filter.PageSize.Value)
                .Take(filter.PageSize.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }
    
    public async Task<TaskItem?> GetByIdIncludingProjectSubtasksTagsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.TaskItems
            .Include(ti => ti.Project)
            .Include(ti => ti.SubTaskItems)
            .Include(ti => ti.Tags)
            .FirstOrDefaultAsync(ti => ti.Id == id && ti.UserId == userId, cancellationToken);
    }
    
    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbContext.TaskItems.FirstOrDefaultAsync(ti => ti.Id == id && ti.UserId == userId, cancellationToken);
    }

    public void Delete(TaskItem taskItem)
    {
        DbContext.TaskItems.Remove(taskItem);
    }
}
