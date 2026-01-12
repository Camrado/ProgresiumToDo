using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Application.Tasks.Repositories.DTOs;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.Queries.GetTasks;

internal sealed class GetTasksQueryHandler : IQueryHandler<GetTasksQuery, GetTasksQueryResponse>
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserContext _userContext;
    
    public GetTasksQueryHandler(ITaskItemRepository taskItemRepository, IUserContext userContext)
    {
        _taskItemRepository = taskItemRepository;
        _userContext = userContext;
    }
    
    public async Task<Result<GetTasksQueryResponse>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
    {
        OrderType? orderType = string.IsNullOrEmpty(request.OrderType) ? null : Enum.Parse<OrderType>(request.OrderType, ignoreCase: true);
        var taskQueryFilter = new TaskQueryFilter(
            _userContext.UserId, request.ProjectId, request.DueDateFrom, request.DueDateTo, orderType,
            request.Page, request.PageSize, request.SortBy, request.SortOrder);

        var result = await _taskItemRepository.GetAllByUserIdIncludingProjectSubtasksTagsAsync(taskQueryFilter, cancellationToken);

        var taskResponses = result.Select(item => new TaskListItemDto(
            item.TaskItem.Id,
            item.TaskItem.Title,
            item.TaskItem.Priority.ToString(),
            item.TaskItem.ClosedAt,
            item.TaskItem.Status.ToString(),
            item.TaskItem.Tags.Select(t => t.Name).ToList(),
            item.Subtasks.Select(sti => new SubTaskListItemDto(sti.SubtaskItem.Id, sti.SubtaskItem.Title, sti.SubtaskItem.Status.ToString(), sti.OrderIndex)).ToList(),
            item.TaskItem.ProjectId,
            item.TaskItem.Project?.Name,
            item.TaskItem.DueDate,
            item.OrderIndex
            ))
            .ToList();
        
        return new GetTasksQueryResponse("Tasks retrieved successfully.", taskResponses);
    }
}