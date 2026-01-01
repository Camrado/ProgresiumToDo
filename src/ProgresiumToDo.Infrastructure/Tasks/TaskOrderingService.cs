using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Infrastructure.Tasks;

internal sealed class TaskOrderingService : ITaskOrderingService
{
    private const int OrderIncrement = 10;
    
    private readonly ITaskOrderRepository _taskOrderRepository;
    
    public TaskOrderingService(ITaskOrderRepository taskOrderRepository)
    {
        _taskOrderRepository = taskOrderRepository;
    }
    
    public async Task CreateInitialOrdersAsync(TaskItem taskItem, Guid? projectId, DateOnly? dueDate,
        CancellationToken cancellationToken)
    {
        if (projectId.HasValue)
        {
            var maxOrderIndex = await _taskOrderRepository
                .GetMaxOrderIndexByProjectAsync(projectId.Value, cancellationToken);

            var taskOrder = TaskOrder.Create(taskItem.Id, OrderType.ByProject, maxOrderIndex + OrderIncrement,
                projectId, null, null);
            _taskOrderRepository.Add(taskOrder);
        }

        if (dueDate.HasValue)
        {
            var maxOrderIndex = await _taskOrderRepository
                .GetMaxOrderIndexByDueDateAsync(dueDate.Value, cancellationToken);
                
            var taskOrder = TaskOrder.Create(taskItem.Id, OrderType.ByDueDate, maxOrderIndex + OrderIncrement, 
                null, dueDate, null);
            _taskOrderRepository.Add(taskOrder);
        }
    }

    public async Task RecalculateOrdersAsync(Guid taskId, Guid? newProjectId, DateOnly? newDueDate, CancellationToken cancellationToken)
    {
        if (newProjectId.HasValue)
        {
            var taskOrder = await _taskOrderRepository
                .GetByTaskIdAndOrderTypeAsync(taskId, OrderType.ByProject, cancellationToken);

            var maxOrderIndex = await _taskOrderRepository
                .GetMaxOrderIndexByProjectAsync(newProjectId.Value, cancellationToken);

            taskOrder.UpdateProjectId(newProjectId.Value);
            taskOrder.UpdateOrderIndex(maxOrderIndex + OrderIncrement);
        }

        if (newDueDate.HasValue)
        {
            var taskOrder = await _taskOrderRepository
                .GetByTaskIdAndOrderTypeAsync(taskId, OrderType.ByDueDate, cancellationToken);

            var maxOrderIndex = await _taskOrderRepository
                .GetMaxOrderIndexByDueDateAsync(newDueDate.Value, cancellationToken);

            taskOrder.UpdateDueDate(newDueDate.Value);
            taskOrder.UpdateOrderIndex(maxOrderIndex + OrderIncrement);
        }
    }

    public async Task ApplyStatusChangeAsync(TaskStatus newStatus, TaskItem taskItem, CancellationToken cancellationToken)
    {
        if (newStatus == TaskStatus.Completed || newStatus == TaskStatus.Cancelled)
        {
            var taskOrders = await _taskOrderRepository.GetByTaskId(taskItem.Id, cancellationToken);
            _taskOrderRepository.DeleteRange(taskOrders);
        }
        else
        {
            await CreateInitialOrdersAsync(taskItem, taskItem.ProjectId, taskItem.DueDate, cancellationToken);
        }
    }
}