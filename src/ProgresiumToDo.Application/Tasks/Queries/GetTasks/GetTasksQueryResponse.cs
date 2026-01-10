namespace ProgresiumToDo.Application.Tasks.Queries.GetTasks;

public sealed record GetTasksQueryResponse(
    string Message,
    IEnumerable<TaskListItemDto> Tasks
    );
    
public sealed record TaskListItemDto(
    Guid Id, 
    string Title, 
    string Priority,
    DateTime? ClosedAt,
    string Status,
    IEnumerable<string> Tags,
    IEnumerable<SubTaskListItemDto> SubTasks,
    string ProjectName,
    DateOnly? DueDate,
    decimal? OrderIndex
    );
    
public sealed record SubTaskListItemDto(
    Guid Id,
    string Title,
    string Status,
    decimal? OrderIndex
    );