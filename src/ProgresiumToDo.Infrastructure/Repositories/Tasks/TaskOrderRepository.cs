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

    public async Task<decimal?> GetNextOrderIndexAsync(OrderType orderType, Guid? projectId, DateOnly? dueDate, Guid? parentTaskId,
        CancellationToken cancellationToken = default)
    {
        if (orderType == OrderType.ByDueDate)
        {
            if (!dueDate.HasValue)
                return null;

            var query = _dbContext.TaskOrders
                .Where(to => to.OrderType == OrderType.ByDueDate && to.DueDate == dueDate.Value);

            var maxOrderIndex = await query.AnyAsync(cancellationToken) 
                ? await query.MaxAsync(to => to.OrderIndex, cancellationToken) 
                : 0;
            
            return maxOrderIndex + 10;
        } 
        
        if (orderType == OrderType.ByProject)
        {
            if (!projectId.HasValue)
                return null;

            var query = _dbContext.TaskOrders
                .Where(to => to.OrderType == OrderType.ByProject && to.ProjectId == projectId.Value);
            
            var maxOrderIndex = await query.AnyAsync(cancellationToken) 
                ? await query.MaxAsync(to => to.OrderIndex, cancellationToken) 
                : 0;
            
            return maxOrderIndex + 10;
        }

        return null;
    }
}