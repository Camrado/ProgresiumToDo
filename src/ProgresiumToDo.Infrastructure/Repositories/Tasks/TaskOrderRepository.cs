using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Infrastructure.Repositories.Tasks;

internal sealed class TaskOrderRepository : ITaskOrderRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public TaskOrderRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Add(TaskOrder taskOrder)
    {
        _dbContext.TaskOrders.Add(taskOrder);
    }
    
    public void Delete(TaskOrder taskOrder)
    {
        _dbContext.TaskOrders.Remove(taskOrder);
    }
    
    public void DeleteRange(IEnumerable<TaskOrder> taskOrders)
    {
        _dbContext.TaskOrders.RemoveRange(taskOrders);
    }

    public async Task<TaskOrder> GetByTaskIdAndOrderTypeAsync(Guid taskId, OrderType orderType, bool trackChanges = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TaskOrder> query = _dbContext.TaskOrders;

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .Where(to => to.OrderType == orderType && to.TaskId == taskId)
            .OrderByDescending(to => to.OrderIndex)
            .FirstAsync(cancellationToken);
    }

    public async Task<List<TaskOrder>> GetByTaskId(Guid taskId, bool trackChanges = false, CancellationToken cancellationToken = default)
    {
        IQueryable<TaskOrder> query = _dbContext.TaskOrders;

        if (!trackChanges)
            query = query.AsNoTracking();

        return await query
            .Where(to => to.TaskId == taskId)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetMaxOrderIndexByDueDateAsync(DateOnly dueDate, CancellationToken cancellationToken)
    {
        var query = _dbContext.TaskOrders
            .Where(to => to.OrderType == OrderType.ByDueDate && to.DueDate == dueDate);

        return await query.AnyAsync(cancellationToken) 
            ? await query.MaxAsync(to => to.OrderIndex, cancellationToken) 
            : 0;
    }

    public async Task<decimal> GetMaxOrderIndexByProjectAsync(Guid projectId, CancellationToken cancellationToken)
    {
        var query = _dbContext.TaskOrders
            .Where(to => to.OrderType == OrderType.ByProject && to.ProjectId == projectId);

        return await query.AnyAsync(cancellationToken) 
            ? await query.MaxAsync(to => to.OrderIndex, cancellationToken) 
            : 0;
    }
    
    public async Task<decimal> GetMaxOrderIndexByParentTaskAsync(Guid parentTaskId, CancellationToken cancellationToken)
    {
        var query = _dbContext.TaskOrders
            .Where(to => to.OrderType == OrderType.ByParentTask && to.ParentTaskId == parentTaskId);

        return await query.AnyAsync(cancellationToken) 
            ? await query.MaxAsync(to => to.OrderIndex, cancellationToken) 
            : 0;
    }
}