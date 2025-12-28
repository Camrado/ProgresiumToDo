namespace ProgresiumToDo.Application.Tasks.GetTasks;

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
    DateOnly? DueDate
    );
    
public sealed record SubTaskListItemDto(
    Guid Id,
    string Title,
    string Status
    );