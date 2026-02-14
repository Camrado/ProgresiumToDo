namespace ProgresiumToDo.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommandResponse(
    string Message,
    CreatedTaskDto CreatedTask);

public sealed record CreatedTaskDto(
    Guid Id,
    Guid? ProjectId,
    string Title,
    string? Description,
    string Priority,
    DateOnly? DueDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string Status, 
    IEnumerable<string> Tags,
    DateTime CreatedAt);
