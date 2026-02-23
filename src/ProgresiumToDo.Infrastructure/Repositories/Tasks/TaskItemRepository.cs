using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Application.Tasks.Repositories.DTOs;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Infrastructure.Repositories.Tasks;

internal sealed class TaskItemRepository : Repository<TaskItem>, ITaskItemRepository
{
    public TaskItemRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<TaskItemWithOrder>> GetAllByUserIdIncludingProjectSubtasksTagsAsync(TaskQueryFilter filter, CancellationToken cancellationToken = default)
    {
        var query = DbContext.TaskItems.AsNoTracking();

        query = query.Where(ti => ti.UserId == filter.UserId);

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
            // Join with TaskOrders to get the order index for each task item
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

        var taskItems = await query.ToListAsync(cancellationToken);
        
        var taskItemsWithSubtasksWithOrder = await ApplySubtaskOrderingAsync(taskItems, cancellationToken);

        return taskItemsWithSubtasksWithOrder
            .Select(ti => ti with { OrderIndex = taskOrderMap?.GetValueOrDefault(ti.TaskItem.Id) ?? null })
            .ToList();
    }

    private async Task<List<TaskItemWithOrder>> ApplySubtaskOrderingAsync(
        List<TaskItem> taskItems, CancellationToken cancellationToken)
    {
        var parentTaskIds = taskItems.Select(t => t.Id).ToList();

        var subtaskOrders = await DbContext.TaskOrders
            .Where(to => to.OrderType == OrderType.ByParentTask && parentTaskIds.Contains(to.ParentTaskId!.Value))
            .Select(to => new { to.TaskId, to.ParentTaskId, to.OrderIndex })
            .ToListAsync(cancellationToken);

        var subtaskOrderLookup = subtaskOrders
            .GroupBy(so => so.ParentTaskId)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(x => x.TaskId, x => x.OrderIndex));

        var result = taskItems.Select(taskItem =>
        {
            var subtasks = subtaskOrderLookup.TryGetValue(taskItem.Id, out var subtasksOrder)
                ? taskItem.SubTaskItems
                    .Select(st =>
                    {
                        var subtaskOrder = subtasksOrder.TryGetValue(st.Id, out var orderIndex) ? (decimal?)orderIndex : null;
                        return new SubtaskItemWithOrder(st, subtaskOrder);
                    })
                    .OrderBy(st => st.OrderIndex)
                    .ToList()
                : [];

            return new TaskItemWithOrder(taskItem, null, subtasks);
        });

        return result.ToList();
    }
    
    public async Task<TaskItemWithOrder?> GetByIdIncludingProjectSubtasksTagsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var query = DbContext.TaskItems.AsNoTracking();

        var taskItem = await query
            .Include(ti => ti.Project)
            .Include(ti => ti.SubTaskItems)
            .Include(ti => ti.Tags)
            .FirstOrDefaultAsync(ti => ti.Id == id && ti.UserId == userId, cancellationToken);

        var result = new TaskItemWithOrder(taskItem, null, null);
        
        if (taskItem is not null)
        {
            var taskItemWithSubtasksWithOrder = await ApplySubtaskOrderingAsync([taskItem], cancellationToken);
            result = taskItemWithSubtasksWithOrder[0];
        }

        return result;
    }
    
    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = DbContext.TaskItems;

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(ti => ti.Id == id && ti.UserId == userId, cancellationToken);
    }
    
    public async Task<TaskItem?> GetByIdWithTagsIncludedAsync(Guid id, Guid userId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TaskItem> query = DbContext.TaskItems;

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .Include(ti => ti.Tags)
            .FirstOrDefaultAsync(ti => ti.Id == id && ti.UserId == userId, cancellationToken);
    }

    public void Delete(TaskItem taskItem)
    {
        DbContext.TaskItems.Remove(taskItem);
    }
}
