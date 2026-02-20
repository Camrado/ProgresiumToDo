using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;
using TaskStatus = ProgresiumToDo.Domain.Tasks.TaskStatus;

namespace ProgresiumToDo.Application.Tasks.Commands.UpdateSubtask;

internal sealed class UpdateSubtaskCommandHandler : ICommandHandler<UpdateSubtaskCommand, UpdateSubtaskCommandResponse>
{
    private readonly ITaskStatusPolicy _taskStatusPolicy;
    private readonly ITaskOrderingService _taskOrderingService;
    
    public UpdateSubtaskCommandHandler(ITaskStatusPolicy taskStatusPolicy, ITaskOrderingService taskOrderingService)
    {
        _taskStatusPolicy = taskStatusPolicy;
        _taskOrderingService = taskOrderingService;
    }
    
    public async Task<Result<UpdateSubtaskCommandResponse>> Handle(UpdateSubtaskCommand request, CancellationToken cancellationToken)
    {
        var oldStatus = request.SubtaskItem!.Status;
        
        request.SubtaskItem!.UpdateSubtask(request.Title, request.Description, request.StartTime, request.EndTime,
            request.Priority, request.Status);
        
        var hasStatusChanged = Enum.TryParse<TaskStatus>(request.Status, ignoreCase: true, out var newStatus);
        
        // Task order recalculation logic
        // 1. Update order if OrderIndex is provided. Case when user wants to change order explicitly.
        if (request.NextTaskOrderIndex.HasValue || request.PreviousTaskOrderIndex.HasValue)
        {
            var orderContext = new TaskOrderContext
            {
                OrderType = OrderType.ByParentTask,
                ParentTaskId = request.SubtaskItem.ParentTaskItemId,
                PreviousTaskOrderIndex = request.PreviousTaskOrderIndex,
                NextTaskOrderIndex = request.NextTaskOrderIndex
            };
            await _taskOrderingService.UpdateOrderAsync(request.SubtaskId, orderContext, cancellationToken);
        }
        // 2. Apply status change if logical group has changed.
        // Case when task is moved between logical groups like InProgress -> Finished.
        else if (hasStatusChanged && _taskStatusPolicy.HasLogicalGroupChanged(oldStatus, newStatus))
        {
            await _taskOrderingService.ApplyStatusChangeAsync(newStatus, request.SubtaskItem, cancellationToken);
        }
        
        return new UpdateSubtaskCommandResponse("Subtask updated successfully.");
    }
}