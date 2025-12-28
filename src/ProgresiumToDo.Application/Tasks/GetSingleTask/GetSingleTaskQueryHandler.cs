using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tasks.GetSingleTask;

internal sealed class GetSingleTaskQueryHandler : IQueryHandler<GetSingleTaskQuery, GetSingleTaskQueryResponse>
{
    public Task<Result<GetSingleTaskQueryResponse>> Handle(GetSingleTaskQuery request, CancellationToken cancellationToken)
    {
        var taskItem = request.TaskItem!;

        var taskItemDto = new TaskDetailsDto(
            taskItem.Id,
            taskItem.Title,
            taskItem.Description ?? string.Empty,
            taskItem.Priority.ToString(),
            taskItem.DueDate,
            taskItem.StartTime,
            taskItem.EndTime,
            taskItem.ClosedAt,
            taskItem.Status.ToString(),
            taskItem.Project.Name,
            taskItem.Tags.Select(t => t.Name),
            taskItem.SubTaskItems.Select(subtask => new SubTaskDetailsDto(
                subtask.Id,
                subtask.Title,
                subtask.Description ?? string.Empty,
                subtask.Priority.ToString(),
                subtask.Status.ToString(),
                subtask.StartTime,
                subtask.EndTime,
                subtask.ClosedAt,
                subtask.CreatedAt)),
            taskItem.CreatedAt,
            taskItem.OrderIndex);

        return Task.FromResult<Result<GetSingleTaskQueryResponse>>(
            new GetSingleTaskQueryResponse("Task retrieved successfully.", taskItemDto));
    }
}