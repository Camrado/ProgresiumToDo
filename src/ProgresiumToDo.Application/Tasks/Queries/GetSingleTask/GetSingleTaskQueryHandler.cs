using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Abstractions;

namespace ProgresiumToDo.Application.Tasks.Queries.GetSingleTask;

internal sealed class GetSingleTaskQueryHandler : IQueryHandler<GetSingleTaskQuery, GetSingleTaskQueryResponse>
{
    public Task<Result<GetSingleTaskQueryResponse>> Handle(GetSingleTaskQuery request, CancellationToken cancellationToken)
    {
        var taskItemWithOrder = request.TaskItemWithOrder;
        var taskItem = taskItemWithOrder!.TaskItem;

        var taskItemDto = new TaskDetailsDto(
            taskItem!.Id,
            taskItem.Title,
            taskItem.Description ?? string.Empty,
            taskItem.Priority.ToString(),
            taskItem.DueDate,
            taskItem.StartTime,
            taskItem.EndTime,
            taskItem.ClosedAt,
            taskItem.Status.ToString(),
            taskItem.ProjectId,
            taskItem.Project?.Name,
            taskItem.Tags.Select(t => t.Name),
            taskItemWithOrder.Subtasks?.Select(subtaskWithOrder => new SubTaskDetailsDto(
                subtaskWithOrder.SubtaskItem.Id,
                subtaskWithOrder.SubtaskItem.Title,
                subtaskWithOrder.SubtaskItem.Description ?? string.Empty,
                subtaskWithOrder.SubtaskItem.Priority.ToString(),
                subtaskWithOrder.SubtaskItem.Status.ToString(),
                subtaskWithOrder.SubtaskItem.StartTime,
                subtaskWithOrder.SubtaskItem.EndTime,
                subtaskWithOrder.SubtaskItem.ClosedAt,
                subtaskWithOrder.SubtaskItem.CreatedAt,
                subtaskWithOrder.OrderIndex)) ?? [],
            taskItem.CreatedAt);

        return Task.FromResult<Result<GetSingleTaskQueryResponse>>(
            new GetSingleTaskQueryResponse("Task retrieved successfully.", taskItemDto));
    }
}