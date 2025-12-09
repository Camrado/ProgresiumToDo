namespace ProgresiumToDo.Application.Tasks.CreateTask;

public sealed record CreateTaskCommandResponse(
    string Message,
    TaskResponse Task);

public sealed record TaskResponse(
    Guid Id,
    Guid ProjectId,
    string Title,
    string? Description,
    string Priority,
    DateOnly? DueDate,
    TimeSpan? Duration,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string Status,
    DateTime CreatedAt);
