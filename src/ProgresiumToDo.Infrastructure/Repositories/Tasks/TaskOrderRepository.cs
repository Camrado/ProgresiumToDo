using Microsoft.EntityFrameworkCore;
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

    public async Task<TaskOrder> GetByTaskIdAndOrderTypeAsync(Guid taskId, OrderType orderType,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaskOrders
            .Where(to => to.OrderType == orderType && to.TaskId == taskId)
            .OrderByDescending(to => to.OrderIndex)
            .FirstAsync(cancellationToken);
    }

    public async Task<List<TaskOrder>> GetByTaskId(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.TaskOrders
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
}