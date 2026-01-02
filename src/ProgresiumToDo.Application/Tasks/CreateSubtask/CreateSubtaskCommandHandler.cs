using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.CreateSubtask;

internal sealed class CreateSubtaskCommandHandler : ICommandHandler<CreateSubtaskCommand, CreateSubtaskCommandResponse>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserContext _userContext;
    private readonly ITaskOrderingService _taskOrderingService;
    
    public CreateSubtaskCommandHandler(ITaskItemRepository taskItemRepository, IUserContext userContext, ITaskOrderingService taskOrderingService)
    {
        _taskItemRepository = taskItemRepository;
        _userContext = userContext;
        _taskOrderingService = taskOrderingService;
    }
    
    public async Task<Result<CreateSubtaskCommandResponse>> Handle(CreateSubtaskCommand request, CancellationToken cancellationToken)
    {
        var subTaskItem = TaskItem.CreateSubtask(
            _userContext.UserId,
            request.ParentTaskId,
            request.Title,
            request.Description,
            request.StartTime,
            request.EndTime,
            request.Priority,
            request.Status);
        
        _taskItemRepository.Add(subTaskItem);

        var orderContext = new TaskOrderContext { ParentTaskId = request.ParentTaskId };
        await _taskOrderingService.CreateInitialOrdersAsync(subTaskItem, orderContext, cancellationToken);
        
        var subtaskResponse = new CreatedSubTaskDto(
            subTaskItem.Id,
            request.ParentTaskId,
            subTaskItem.Title,
            subTaskItem.Description,
            subTaskItem.Priority.ToString(),
            subTaskItem.Status.ToString(),
            subTaskItem.StartTime,
            subTaskItem.EndTime,
            subTaskItem.CreatedAt);
        
        return new CreateSubtaskCommandResponse("Subtask created successfully.", subtaskResponse);
    }
}