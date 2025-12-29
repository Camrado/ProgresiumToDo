using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.UpdateTask;

internal sealed class UpdateTaskCommandHandler : ICommandHandler<UpdateTaskCommand, UpdateTaskCommandResponse>
{
    private readonly ITaskItemRepository _taskItemRepository;
    
    public UpdateTaskCommandHandler(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }
    
    public async Task<Result<UpdateTaskCommandResponse>> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
    {
        request.TaskItem!.Update(request.Title, request.Description, request.Status, request.Priority,
            request.DueDate, request.StartTime, request.EndTime, request.OrderIndex, request.ProjectId);

        // Recalculate order index if due date or project changed
        if ((request.DueDate is not null) || (request.ProjectId is not null && request.TaskItem!.DueDate is not null))
        {
            var maxOrderIndex = await _taskItemRepository.GetMaxOrderIndexByProjectId(request.TaskItem!.ProjectId,
                request.TaskItem!.DueDate!.Value, null, cancellationToken);

            request.TaskItem!.UpdateOrderIndex(maxOrderIndex + 10);
        }

        return new UpdateTaskCommandResponse("Task updated successfully.");
    }
}