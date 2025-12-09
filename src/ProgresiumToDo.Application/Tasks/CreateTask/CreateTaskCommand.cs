using ProgresiumToDo.Application.Abstractions.Messaging;
using ProgresiumToDo.Domain.Tasks;

namespace ProgresiumToDo.Application.Tasks.CreateTask;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    string? Priority,
    DateOnly? DueDate,
    TimeSpan? Duration,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Status) : ICommand<CreateTaskCommandResponse>;
