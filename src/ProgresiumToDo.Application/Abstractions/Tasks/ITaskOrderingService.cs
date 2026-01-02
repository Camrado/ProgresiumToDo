using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Abstractions.Tasks;

public interface ITaskOrderingService
{
    Task UpdateOrderAsync(Guid taskId, TaskOrderContext orderContext, decimal newOrderIndex, CancellationToken cancellationToken);
    
    Task CreateInitialOrdersAsync(TaskItem taskItem, TaskOrderContext orderContext, CancellationToken cancellationToken);
    
    Task RecalculateOrdersAsync(Guid taskId, TaskOrderContext orderContext, CancellationToken cancellationToken);

    Task ApplyStatusChangeAsync(TaskStatus newStatus, TaskItem taskItem, CancellationToken cancellationToken);
}