using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Tasks;
using ProgresiumToDo.Domain.Tasks.DTOs;

namespace ProgresiumToDo.Application.Tasks.GetTasks;

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
        var taskQueryFilter = new TaskQueryFilter(
            _userContext.UserId, request.ProjectId, request.DueDateFrom, request.DueDateTo,
            request.Page, request.PageSize, request.SortBy, request.SortOrder);

        var tasks = await _taskItemRepository.GetByUserIdIncludingProjectSubtasksTagsAsync(taskQueryFilter, cancellationToken);

        var taskResponses = tasks.Select(taskItem => new TaskListItemDto(
            taskItem.Id,
            taskItem.Title,
            (taskItem.Priority ?? Priority.None).ToString(),
            taskItem.ClosedAt,
            taskItem.Status.ToString(),
            taskItem.Tags.Select(t => t.Name).ToList(),
            taskItem.SubTaskItems.Select(sti => new SubTaskListItemDto(sti.Id, sti.Title, sti.Status.ToString())).ToList(),
            taskItem.Project.Name,
            taskItem.DueDate
            ))
            .ToList();
        
        return new GetTasksQueryResponse("Tasks retrieved successfully.", taskResponses);
    }
}