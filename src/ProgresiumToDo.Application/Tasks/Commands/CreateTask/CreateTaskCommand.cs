using ProgresiumToDo.Application.Abstractions.Messaging;

namespace ProgresiumToDo.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid? ProjectId,
    string Title,
    string? Description,
    string? Priority,
    DateOnly? DueDate,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Status) : ICommand<CreateTaskCommandResponse>;
