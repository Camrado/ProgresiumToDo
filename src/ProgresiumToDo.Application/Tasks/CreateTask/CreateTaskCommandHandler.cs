using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.CreateTask;

internal sealed class CreateTaskCommandHandler : ICommandHandler<CreateTaskCommand, CreateTaskCommandResponse>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly ITaskOrderRepository _taskOrderRepository;
    private readonly IUserContext _userContext;

    public CreateTaskCommandHandler(ITaskItemRepository taskItemRepository, ITaskOrderRepository taskOrderRepository, IUserContext userContext)
    {
        _taskItemRepository = taskItemRepository;
        _taskOrderRepository = taskOrderRepository;
        _userContext = userContext;
    }

    public async Task<Result<CreateTaskCommandResponse>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var taskItem = TaskItem.Create(
            request.ProjectId,
            _userContext.UserId,
            request.Title,
            request.Description,
            request.Status,
            request.Priority,
            request.DueDate,
            request.StartTime,
            request.EndTime);

        _taskItemRepository.Add(taskItem);
        
        // Create TaskOrder entries for all OrderTypes
        foreach (OrderType orderType in Enum.GetValues(typeof(OrderType)))
        {
            var nextOrderIndex = await _taskOrderRepository.GetNextOrderIndexAsync(orderType, request.ProjectId,
                request.DueDate, null, cancellationToken);
            
            if (nextOrderIndex.HasValue)
            {
                var taskOrder = TaskOrder.Create(taskItem.Id, orderType, nextOrderIndex.Value, request.ProjectId,
                    request.DueDate, null);
                _taskOrderRepository.Add(taskOrder);
            }
        }

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
