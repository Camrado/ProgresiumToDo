namespace ProgresiumToDo.Application.Tasks.GetSingleTask;

public sealed record GetSingleTaskQueryResponse(
    string Message,
    TaskDetailsDto Task);
    
public sealed record TaskDetailsDto(
    Guid Id,
    string Title,
    string Description,
    string Priority,
    DateOnly? DueDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    DateTime? ClosedAt,
    string Status,
    string ProjectName,
    IEnumerable<string> Tags,
    IEnumerable<SubTaskDetailsDto> SubTasks,
    DateTime CreatedAt);
    
public sealed record SubTaskDetailsDto(
    Guid Id,
    string Title,
    string Description,
    string Priority,
    string Status,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    DateTime? ClosedAt,
    DateTime CreatedAt);