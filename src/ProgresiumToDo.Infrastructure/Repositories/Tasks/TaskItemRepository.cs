using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Tasks;
using ProgresiumToDo.Domain.Tasks.DTOs;

namespace ProgresiumToDo.Infrastructure.Repositories.Tasks;

internal sealed class TaskItemRepository : Repository<TaskItem>, ITaskItemRepository
{
    public TaskItemRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<TaskItemWithOrder>> GetAllByUserIdIncludingProjectSubtasksTagsAsync(TaskQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var query = DbContext.TaskItems
            .AsNoTracking()
            .Where(ti => ti.UserId == filter.UserId);

        if (filter.ProjectId.HasValue && filter.ProjectId != Guid.Empty)
            query = query.Where(ti => ti.ProjectId == filter.ProjectId.Value);
        else if (filter.ProjectId == Guid.Empty)
            query = query.Where(ti => ti.ProjectId == null);
    
        if (filter.DueDateFrom.HasValue)
            query = query.Where(ti => ti.DueDate >= filter.DueDateFrom.Value);
    
        if (filter.DueDateTo.HasValue)
            query = query.Where(ti => ti.DueDate <= filter.DueDateTo.Value);

        Dictionary<Guid, decimal>? taskOrderMap = null;
        
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
                _ => query.OrderBy(ti => ti.Id)
            };
        }
        else if (filter.OrderType.HasValue)
        {
            var queryWithOrder = query
                .GroupJoin(
                    DbContext.TaskOrders.Where(to => to.OrderType == filter.OrderType.Value),
                    task => task.Id,
                    order => order.TaskId,
                    (task, orders) => new { task, orders })
                .SelectMany(
                    x => x.orders.DefaultIfEmpty(),
                    (x, order) => new { x.task, orderIndex = order != null ? order.OrderIndex : int.MaxValue })
                .OrderBy(x => x.orderIndex);
            
            var pagedQuery = filter.Page.HasValue && filter.PageSize.HasValue
                ? queryWithOrder.Skip((filter.Page.Value - 1) * filter.PageSize.Value).Take(filter.PageSize.Value)
                : queryWithOrder;

            query = queryWithOrder.Select(qwo => qwo.task);

            taskOrderMap = pagedQuery.ToDictionary(qwo => qwo.task.Id, qwo => qwo.orderIndex);
        }
        else
        {
            query = query.OrderBy(ti => ti.Id);
        }
        
        query = query
            .Include(ti => ti.Project)
            .Include(ti => ti.SubTaskItems)
            .Include(ti => ti.Tags);
        
        if (filter is { Page: not null, PageSize: not null })
        {
            query = query
                .Skip((filter.Page.Value - 1) * filter.PageSize.Value)
                .Take(filter.PageSize.Value);
        }

        var result = await query.ToListAsync(cancellationToken);
        
        return result.Select(ti => new TaskItemWithOrder(ti, taskOrderMap?.GetValueOrDefault(ti.Id) ?? null)).ToList();
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
