namespace ProgresiumToDo.Application.Tasks.CreateTask;

public sealed record CreateTaskCommandResponse(
    string Message,
    CreatedTaskResponse CreatedTask);

public sealed record CreatedTaskResponse(
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
