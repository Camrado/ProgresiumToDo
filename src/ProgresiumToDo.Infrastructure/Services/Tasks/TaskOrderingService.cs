using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Infrastructure.Services.Tasks;

internal sealed class TaskOrderingService : ITaskOrderingService
{
    private const int OrderIncrement = 10;
    
    private readonly ITaskOrderRepository _taskOrderRepository;
    
    public TaskOrderingService(ITaskOrderRepository taskOrderRepository)
    {
        _taskOrderRepository = taskOrderRepository;
    }
    
    public async Task UpdateOrderAsync(Guid taskId, TaskOrderContext orderContext, decimal newOrderIndex,
        CancellationToken cancellationToken)
    {
        var taskOrder = await _taskOrderRepository
            .GetByTaskIdAndOrderTypeAsync(taskId, orderContext.OrderType, trackChanges: true, cancellationToken);

        if (orderContext.OrderType == OrderType.ByProject)
        {
            taskOrder.UpdateProjectId(orderContext.ProjectId);
        }
        else if (orderContext.OrderType == OrderType.ByDueDate)
        {
            taskOrder.UpdateDueDate(orderContext.DueDate);
        }
        else if (orderContext.OrderType == OrderType.ByParentTask)
        {
            taskOrder.UpdateParentTaskId(orderContext.ParentTaskId);
        }

        taskOrder.UpdateOrderIndex(newOrderIndex);
    }
    
    public async Task CreateInitialOrdersAsync(TaskItem taskItem, TaskOrderContext orderContext,
        CancellationToken cancellationToken)
    {
        if (orderContext.ProjectId is { } projectId)
        {
            var maxOrderIndex = await _taskOrderRepository
                .GetMaxOrderIndexByProjectAsync(projectId, cancellationToken);

            var taskOrder = TaskOrder.Create(taskItem.Id, OrderType.ByProject, maxOrderIndex + OrderIncrement,
                projectId, null, null);
            _taskOrderRepository.Add(taskOrder);
        }

        if (orderContext.DueDate is { } dueDate)
        {
            var maxOrderIndex = await _taskOrderRepository
                .GetMaxOrderIndexByDueDateAsync(dueDate, cancellationToken);
                
            var taskOrder = TaskOrder.Create(taskItem.Id, OrderType.ByDueDate, maxOrderIndex + OrderIncrement, 
                null, dueDate, null);
            _taskOrderRepository.Add(taskOrder);
        }
        
        if (orderContext.ParentTaskId is { } parentTaskId)
        {
            var maxOrderIndex = await _taskOrderRepository
                .GetMaxOrderIndexByParentTaskAsync(parentTaskId, cancellationToken);
                
            var taskOrder = TaskOrder.Create(taskItem.Id, OrderType.ByParentTask, maxOrderIndex + OrderIncrement, 
                null, null, parentTaskId);
            _taskOrderRepository.Add(taskOrder);
        }
    }

    public async Task RecalculateOrdersAsync(Guid taskId, TaskOrderContext orderContext, CancellationToken cancellationToken)
    {
        if (orderContext.ProjectId is { } newProjectId)
        {
            var taskOrder = await _taskOrderRepository
                .GetByTaskIdAndOrderTypeAsync(taskId, OrderType.ByProject, trackChanges: true, cancellationToken);

            var maxOrderIndex = await _taskOrderRepository
                .GetMaxOrderIndexByProjectAsync(newProjectId, cancellationToken);

            taskOrder.UpdateProjectId(newProjectId);
            taskOrder.UpdateOrderIndex(maxOrderIndex + OrderIncrement);
        }

        if (orderContext.DueDate is { } newDueDate)
        {
            var taskOrder = await _taskOrderRepository
                .GetByTaskIdAndOrderTypeAsync(taskId, OrderType.ByDueDate, trackChanges: true, cancellationToken);

            var maxOrderIndex = await _taskOrderRepository
                .GetMaxOrderIndexByDueDateAsync(newDueDate, cancellationToken);

            taskOrder.UpdateDueDate(newDueDate);
            taskOrder.UpdateOrderIndex(maxOrderIndex + OrderIncrement);
        }
    }

    public async Task ApplyStatusChangeAsync(TaskStatus newStatus, TaskItem taskItem, CancellationToken cancellationToken)
    {
        if (newStatus == TaskStatus.Completed || newStatus == TaskStatus.Cancelled)
        {
            var taskOrders = await _taskOrderRepository.GetByTaskId(taskItem.Id, trackChanges: true, cancellationToken);
            _taskOrderRepository.DeleteRange(taskOrders);
        }
        else
        {
            TaskOrderContext orderContext;

            // It means it's a top-level task
            if (taskItem.ParentTaskItemId is null)
            {
                orderContext = new TaskOrderContext
                {
                    ProjectId = taskItem.ProjectId,
                    DueDate = taskItem.DueDate
                };
            }
            // It's a sub-task
            else
            {
                orderContext = new TaskOrderContext
                {
                    ParentTaskId = taskItem.ParentTaskItemId
                };
            }

            await CreateInitialOrdersAsync(taskItem, orderContext, cancellationToken);
        }
    }
}