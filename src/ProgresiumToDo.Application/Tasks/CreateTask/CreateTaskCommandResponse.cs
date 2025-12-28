namespace ProgresiumToDo.Application.Tasks.CreateTask;

public sealed record CreateTaskCommandResponse(
    string Message,
    CreatedTaskDto CreatedTask);

public sealed record CreatedTaskDto(
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
