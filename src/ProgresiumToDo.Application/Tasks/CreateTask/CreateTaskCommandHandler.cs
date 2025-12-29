using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.CreateTask;

internal sealed class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, CreateTaskCommandResponse>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserContext _userContext;

    public CreateTaskCommandHandler(ITaskItemRepository taskItemRepository, IUserContext userContext)
    {
        _taskItemRepository = taskItemRepository;
        _userContext = userContext;
    }

    public async Task<Result<CreateTaskCommandResponse>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        decimal? orderIndex = null;

        // ProjectId is required field, so we are checking only for DueDate to set orderIndex
        if (request.DueDate.HasValue)
        {
            orderIndex = await _taskItemRepository.GetMaxOrderIndexByProjectId(
                request.ProjectId, request.DueDate.Value, null, cancellationToken) + 10;
        }
        
        var taskItem = TaskItem.Create(
            request.ProjectId,
            _userContext.UserId,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DueDate,
            request.StartTime,
            request.EndTime,
            orderIndex);

        _taskItemRepository.Add(taskItem);

        var taskResponse = new CreatedTaskDto(
            taskItem.Id,
            taskItem.ProjectId,
            taskItem.Title,
            taskItem.Description,
            taskItem.Priority.ToString(),
            taskItem.DueDate,
            taskItem.StartTime,
            taskItem.EndTime,
            taskItem.Status.ToString(),
            taskItem.CreatedAt);

        return new CreateTaskCommandResponse("CreatedTask created successfully.", taskResponse);
    }
}
