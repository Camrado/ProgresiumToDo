namespace ProgresiumToDo.Application.Tasks.Commands.CreateSubtask;

public sealed record CreateSubtaskCommandResponse(string Message, CreatedSubTaskDto CreatedSubTask);

public sealed record CreatedSubTaskDto(
    Guid Id,
    Guid ParentTaskId,
    string Title,
    string? Description,
    string Priority,
    string Status,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    DateTime CreatedAt);
