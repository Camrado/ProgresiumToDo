using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Domain.Abstractions;
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
        if ((!hasStatusChanged && oldStatus != TaskStatus.Completed && oldStatus != TaskStatus.Cancelled) ||
            (hasStatusChanged && _taskStatusPolicy.HasLogicalGroupStayedAsInProgress(oldStatus, newStatus)))
            await _taskOrderingService.RecalculateOrdersAsync(request.TaskItem.Id, request.ProjectId, request.DueDate, cancellationToken);
        else if (hasStatusChanged && _taskStatusPolicy.HasLogicalGroupChanged(oldStatus, newStatus))
            await _taskOrderingService.ApplyStatusChangeAsync(newStatus, request.TaskItem, cancellationToken);
            
        return new UpdateTaskCommandResponse("Task updated successfully.");
    }
}