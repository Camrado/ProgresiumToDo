using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Abstractions.Tasks;

public interface ITaskOrderingService
{
    Task CreateInitialOrdersAsync(TaskItem taskItem, Guid? projectId, DateOnly? dueDate, CancellationToken cancellationToken);
    
    Task RecalculateOrdersAsync(Guid taskId, Guid? projectId, DateOnly? dueDate, CancellationToken cancellationToken);

    Task ApplyStatusChangeAsync(TaskStatus newStatus, TaskItem taskItem, CancellationToken cancellationToken);
}