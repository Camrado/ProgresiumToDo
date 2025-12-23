using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Tasks.GetTasks;

public sealed record GetTasksQuery(
    DateOnly DueDateFrom,
    DateOnly DueDateTo,
    Guid? ProjectId,
    int? Page,
    int? PageSize,
    string? SortBy,
    string SortOrder = "ASC") : IQuery<GetTasksQueryResponse>;