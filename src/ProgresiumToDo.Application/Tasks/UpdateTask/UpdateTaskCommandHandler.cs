using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Tasks.UpdateTask;

internal sealed class UpdateTaskCommandHandler : ICommandHandler<UpdateTaskCommand, UpdateTaskCommandResponse>
{
    private readonly ITaskStatusPolicy _taskStatusPolicy;
    private readonly ITaskOrderingService _taskOrderingService;
    
    public UpdateTaskCommandHandler(ITaskStatusPolicy taskStatusPolicy, ITaskOrderingService taskOrderingService)
    {
        _taskStatusPolicy = taskStatusPolicy;
        _taskOrderingService = taskOrderingService;
    }
    
    public async Task<Result<UpdateTaskCommandResponse>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        var oldStatus = request.TaskItem!.Status;
        
        request.TaskItem!.Update(request.Title, request.Description, request.Status, request.Priority,
            request.DueDate, request.StartTime, request.EndTime, request.ProjectId);
        
        var hasStatusChanged = Enum.TryParse<TaskStatus>(request.Status, ignoreCase: true, out var newStatus);
        
        // Task order recalculation logic
        // 1. Update order if OrderIndex and OrderType are provided. Case when user wants to change order explicitly.
        if (request.OrderIndex.HasValue && !string.IsNullOrEmpty(request.OrderType))
        {
            var orderContext = new TaskOrderContext
            {
                OrderType = Enum.Parse<OrderType>(request.OrderType, ignoreCase: true),
                DueDate = request.DueDate,
                ProjectId = request.ProjectId
            };
            await _taskOrderingService.UpdateOrderAsync(request.TaskItem.Id, orderContext, request.OrderIndex.Value, cancellationToken);
        }
        // 2. Recalculate orders if status hasn't changed and task is not completed or cancelled,
        // or if status changed but stayed in the "In Progress" logical group. 
        // Case when other fields are updated that might affect ordering.
        else if ((!hasStatusChanged && oldStatus != TaskStatus.Completed && oldStatus != TaskStatus.Cancelled) ||
            (hasStatusChanged && _taskStatusPolicy.HasLogicalGroupStayedAsInProgress(oldStatus, newStatus)))
        {
            var orderContext = new TaskOrderContext
            {
                DueDate = request.DueDate,
                ProjectId = request.ProjectId
            };
            await _taskOrderingService.RecalculateOrdersAsync(request.TaskItem.Id, orderContext, cancellationToken);
        }
        // 3. Apply status change if logical group has changed.
        // Case when task is moved between logical groups like InProgress -> Finished.
        else if (hasStatusChanged && _taskStatusPolicy.HasLogicalGroupChanged(oldStatus, newStatus))
        {
            await _taskOrderingService.ApplyStatusChangeAsync(newStatus, request.TaskItem, cancellationToken);
        }
            
        return new UpdateTaskCommandResponse("Task updated successfully.");
    }
}